﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WDPR_MVC.Areas.Identity.Data;
using WDPR_MVC.Data;
using Microsoft.EntityFrameworkCore;
using GoogleReCaptcha.V3.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace WDPR_MVC.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly MyContext _context;
        private readonly ICaptchaValidator _captchaValidator;
        private readonly IEmailSender _emailSender;

        public LoginModel(SignInManager<ApplicationUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<ApplicationUser> userManager,
            MyContext mycontext,
            ICaptchaValidator captchaValidator,
            IConfiguration configurationKey,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = mycontext;
            _captchaValidator = captchaValidator;
            captchaValidator.UpdateSecretKey(configurationKey["googleReCaptcha:SecretKeyV2"]);
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }
        public ICaptchaValidator CaptchaValidator { get; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            string recaptchaResponse = this.Request.Form["g-recaptcha-response"];
            var captchaPassed = await _captchaValidator.IsCaptchaPassedAsync(recaptchaResponse);

            if (recaptchaResponse != null && !captchaPassed)
            {
                ModelState.AddModelError("captcha", "Captcha validation failed");
            }

            if (ModelState.IsValid)
            {
                //Stuur een emailtje naar een gebruiker als deze inlogt van een ongebruikelijke device of locatie
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Input.Email);
                if (user != null)
                {
                    // Get current ip
                    var currentIp = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    if (Request.Headers.ContainsKey("X-Forwarded-For"))
                    {
                        currentIp = Request.Headers["X-Forwarded-For"].FirstOrDefault();
                    }

                    // Check if current user ip is known
                    var known = user.KnownIps.FirstOrDefault(i => i.Ip == currentIp);

                    // FIXME: Wat gaan we doen als deny?
                    if (known?.Status == KnownIpStatus.Deny)
                    {
                        ModelState.AddModelError(string.Empty, "Not allowed to login with this ip");
                        return Page();
                    }

                    // If unknown send another mail to confirm status?
                    if (known?.Status == KnownIpStatus.Unknown)
                    {
                        var token = await _userManager.GenerateUserTokenAsync(user, "Default", "ip-validation" + currentIp);
                        Console.WriteLine(token);
                    }

                    // User has ip's and this one is not found send email to confirm ip
                    if (user.KnownIps.Any() && known == null)
                    {
                        user.KnownIps.Add(new KnownIp
                        {
                            Ip = currentIp,
                            Status = KnownIpStatus.Unknown
                        });

                        var code = await _userManager.GenerateUserTokenAsync(user, "Default", "ip-validation" + currentIp);
                        Console.WriteLine(code);

                        //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        // TODO: Actually do this KEKW
                        // var callbackUrl = Url.Page(
                        //     "/Account/ConfirmEmail",
                        //     pageHandler: null,
                        //     values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        //     protocol: Request.Scheme);

                        // await _emailSender.SendEmailAsync(user.Email, "New Login From Ip",
                        //     $"We detected a new Ip login please confirm that this was you by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>accepting</a> or <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>denying access</a> (Only valid for 2 days).");
                    }

                    // FIXME: Doesn't have any known ip's should we trust the first one?
                    if (!user.KnownIps.Any())
                    {
                        user.KnownIps.Add(new KnownIp
                        {
                            Ip = currentIp,
                            Status = KnownIpStatus.Allowed
                        });
                    }
                }

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }

                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    var errorMessage = "Invalid login attempt.";
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Input.Email);
                    if (user != null && user?.AccessFailedCount >= 3)
                    {
                        errorMessage += " Bent u uw wachtwoord vergeten?";
                        if (!captchaPassed)
                        {
                            ModelState.AddModelError("captcha", "Captcha validation failed");
                        }
                    }

                    if (user?.AccessFailedCount >= 2)
                    {
                        ViewData["ShowCap"] = true;
                    }

                    ModelState.AddModelError(string.Empty, errorMessage);
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
