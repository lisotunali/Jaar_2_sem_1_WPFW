using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace WDPR_MVC.Authorization
{
    public class MeldingOperations
    {
        public static OperationAuthorizationRequirement Create =
            new OperationAuthorizationRequirement { Name = Constants.CreateOperationName };

        public static OperationAuthorizationRequirement Update =
            new OperationAuthorizationRequirement { Name = Constants.UpdateOperationName };

        public static OperationAuthorizationRequirement Delete =
            new OperationAuthorizationRequirement { Name = Constants.DeleteOperationName };

        public static OperationAuthorizationRequirement Lock =
            new OperationAuthorizationRequirement { Name = Constants.LockOperationName };

        public static OperationAuthorizationRequirement ReadAnonymous =
            new OperationAuthorizationRequirement { Name = Constants.ReadAnonymousOperationName };
    }

    public class Constants
    {
        public static readonly string CreateOperationName = "Create";
        public static readonly string UpdateOperationName = "Update";
        public static readonly string DeleteOperationName = "Delete";
        public static readonly string LockOperationName = "Lock";

        public static readonly string ModeratorRole = "Mod";
        public static readonly string ReadAnonymousOperationName = "ReadAnonymous";
    }
}
