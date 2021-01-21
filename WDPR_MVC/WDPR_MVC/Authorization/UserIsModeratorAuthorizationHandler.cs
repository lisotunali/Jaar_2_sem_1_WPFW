using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using WDPR_MVC.Models;

namespace WDPR_MVC.Authorization
{
    public class UserIsModeratorAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, Melding>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            Melding melding)
        {
            if (context.User == null || melding == null)
            {
                return Task.CompletedTask;
            }

            // Moderators can update, lock and delete
            if (requirement.Name != Constants.UpdateOperationName
                && requirement.Name != Constants.LockOperationName
                && requirement.Name != Constants.DeleteOperationName)
            {
                return Task.CompletedTask;
            }

            if (context.User.IsInRole(Constants.ModeratorRole))
            {
                context.Succeed(requirement);
            }

			// TODO: Mogen moderators bij anonieme meldingen?
            if (melding.IsAnonymous)
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
