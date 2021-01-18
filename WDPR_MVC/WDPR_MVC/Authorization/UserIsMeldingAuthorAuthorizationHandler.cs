using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using WDPR_MVC.Areas.Identity.Data;
using WDPR_MVC.Models;

namespace WDPR_MVC.Authorization
{
    public class UserIsMeldingAuthorAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, Melding>
    {

        UserManager<ApplicationUser> _userManager;

        public UserIsMeldingAuthorAuthorizationHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            Melding melding)
        {
            if (context.User == null || melding == null)
            {
                return Task.CompletedTask;
            }

            // Only continue for Update
            if (requirement.Name != Constants.UpdateOperationName)
            {
                return Task.CompletedTask;
            }

            if (melding.AuteurId == _userManager.GetUserId(context.User))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
