using System;
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
using WDPR_MVC.Models;

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
            var currentIp = getCurrentIp();
            var ip = _context.IPAdressen.FirstOrDefault(i => i.IP.Equals(currentIp));

            if (ip?.FailCount >= 2) ViewData["ShowCap"] = true;

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

            var currentIp = getCurrentIp();
            var ip = await _context.IPAdressen.FirstOrDefaultAsync(i => i.IP.Equals(currentIp));

            if (ip != null && ip.FailCount >= 2)
            {
                var captchaPassed = await _captchaValidator.IsCaptchaPassedAsync(recaptchaResponse);
                if (recaptchaResponse != null && !captchaPassed)
                {
                    ModelState.AddModelError("captcha", "Captcha validation failed");
                    _context.IPAdressen.FirstOrDefault(i => i.IP.Equals(currentIp));
                    ViewData["ShowCap"] = true;
                }
            }

            if (ModelState.IsValid)
            {
                //Stuur een emailtje naar een gebruiker als deze inlogt van een ongebruikelijke device of locatie
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Input.Email);
                if (user != null)
                {
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
                        await sendEmail(user, known, returnUrl);
                    }

                    // User has ip's and this one is not found send email to confirm ip
                    if (user.KnownIps.Any() && known == null)
                    {
                        var newKnownIp = new KnownIp
                        {
                            Ip = currentIp,
                            Status = KnownIpStatus.Unknown
                        };

                        user.KnownIps.Add(newKnownIp);
                        await _context.SaveChangesAsync();
                        await sendEmail(user, newKnownIp, returnUrl);
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
                    if (ip != null)
                    {
                        _context.IPAdressen.Remove(ip);
                        await _context.SaveChangesAsync();
                    }
                    _logger.LogInformation("User logged in.");
                    if (!user.FirstLog)
                    {
                        user.FirstLog = true;
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Uitleg", "Home");   
                    }
                    else 
                    {
                        return Redirect(returnUrl);
                    }
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

                    if (ip == null)
                    {
                        ip = new IPModel { IP = currentIp, FailCount = 0 };
                        await _context.IPAdressen.AddAsync(ip);
                    }

                    if (user != null && user?.AccessFailedCount >= 3)
                    {
                        errorMessage += " Bent u uw wachtwoord vergeten?";
                    }

                    ip.FailCount++;
                    await _context.SaveChangesAsync();

                    if (ip.FailCount >= 2) ViewData["ShowCap"] = true;

                    ModelState.AddModelError(string.Empty, errorMessage);
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private string getCurrentIp()
        {
            // Get current ip
            var currentIp = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                currentIp = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            }

            return currentIp;
        }

        private async Task sendEmail(ApplicationUser user, KnownIp knownIp, string returnUrl)
        {
            var code = await _userManager.GenerateUserTokenAsync(user, "Default", "ip-validation" + knownIp.Ip);

            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrlAccept = Url.Page(
                "/Account/ConfirmIp",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = code, about = knownIp.Id, allow = true, returnUrl = returnUrl },
                protocol: Request.Scheme);

            var callbackUrlDeny = Url.Page(
                "/Account/ConfirmIp",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = code, about = knownIp.Id, allow = false, returnUrl = returnUrl },
                protocol: Request.Scheme);

            // Outlook accepteert dit niet :( dus dan maar alles aan elkaar
            //await _emailSender.SendEmailAsync(user.Email, "We noticed a new sign in to your Buurtje account",
            //    $"Hello {user.UserName},<br><br>We detected a new Ip login attempt on {DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss")} {TimeZoneInfo.Local.DisplayName}.<br><br>please confirm that this was you by <a href='{HtmlEncoder.Default.Encode(callbackUrlAccept)}'>allowing</a> or <a href='{HtmlEncoder.Default.Encode(callbackUrlDeny)}'>denying</a> access to this IP address.<br>(Only valid for 2 days).");

            await _emailSender.SendEmailAsync(user.Email, "We noticed a new sign in to your Buurtje account",
                $"Hello {user.UserName}, We detected a new Ip login attempt on {DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss")} {TimeZoneInfo.Local.DisplayName}. Please confirm that this was you by <a href='{HtmlEncoder.Default.Encode(callbackUrlAccept)}'>allowing</a> or <a href='{HtmlEncoder.Default.Encode(callbackUrlDeny)}'>denying</a> access to this IP address. (Only valid for 2 days).");
        }
    }
}
