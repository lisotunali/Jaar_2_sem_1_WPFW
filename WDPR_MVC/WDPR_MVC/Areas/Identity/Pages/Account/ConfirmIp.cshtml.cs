//using System;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.AspNetCore.WebUtilities;
//using WDPR_MVC.Areas.Identity.Data;
//using WDPR_MVC.Data;
//
//namespace WDPR_MVC.Areas.Identity.Pages.Account
//{
//    [AllowAnonymous]
//    public class ConfirmIpModel : PageModel
//    {
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly MyContext _context;
//
//        public ConfirmIpModel(UserManager<ApplicationUser> userManager, MyContext context)
//        {
//            _userManager = userManager;
//            _context = context;
//        }
//
//        [TempData]
//        public string StatusMessage { get; set; }
//
//        public async Task<IActionResult> OnGetAsync(string userId, string code, int? about, bool? allow)
//        {
//            if (userId == null || code == null || about == null || allow == null)
//            {
//                return RedirectToPage("/Index");
//            }
//
//            var user = await _context.Users.FindAsync(userId);
//            if (user == null)
//            {
//                return NotFound($"Unable to load user with ID '{userId}'.");
//            }
//
//            var knownIp = await _context.KnownIps.FindAsync(about);
//            if (knownIp?.User.Id != userId)
//            {
//                return BadRequest("Invalid request");
//            }
//
//            if (knownIp?.Status == KnownIpStatus.Allowed)
//            {
//                return RedirectToPage("/Index");
//            }
//
//            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
//            var result = await ConfirmIpAsync(user, knownIp.Ip, code);
//            StatusMessage = result.Succeeded ? "Thank you for confirming the IP address." : "Error confirming the IP address.";
//
//            if (result.Succeeded)
//            {
//                // Update the ip status
//                knownIp.Status = allow.Value ? KnownIpStatus.Allowed : KnownIpStatus.Deny;
//                await _context.SaveChangesAsync();
//            }
//
//            return Page();
//        }
//
//        /// <summary>
//        /// Validates that an ip confirmation token matches the specified <paramref name="user"/>.
//        /// </summary>
//        /// <param name="user">The user to validate the token against.</param>
//        /// <param name="ip">The ip the token was generated for.</param>
//        /// <param name="token">The ip confirmation token to validate.</param>
//        /// <returns>
//        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
//        /// of the operation.
//        /// </returns>
//        public virtual async Task<IdentityResult> ConfirmIpAsync(ApplicationUser user, string ip, string token)
//        {
//            if (user == null)
//            {
//                throw new ArgumentNullException(nameof(user));
//            }
//
//            if (!await _userManager.VerifyUserTokenAsync(user, "Default", "ip-validation" + ip, token))
//            {
//                return IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
//            }
//
//            return IdentityResult.Success;
//        }
//    }
//}
