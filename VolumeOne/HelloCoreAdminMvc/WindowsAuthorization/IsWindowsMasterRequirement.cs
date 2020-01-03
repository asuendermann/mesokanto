using Microsoft.AspNetCore.Authorization;

namespace HelloCoreAdminMvc.WindowsAuthorization {
    public class IsWindowsMasterRequirement : IAuthorizationRequirement {
        public const string IsWindowsMasterRequirementPolicy = "IsWindowsMasterRequirementPolicy";
    }
}