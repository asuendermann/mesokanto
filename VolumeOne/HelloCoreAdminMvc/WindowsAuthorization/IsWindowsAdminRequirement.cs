using Microsoft.AspNetCore.Authorization;

namespace HelloCoreAdminMvc.WindowsAuthorization {
    public class IsWindowsAdminRequirement : IAuthorizationRequirement {
        public const string IsWindowsAdminRequirementPolicy = "IsWindowsAdminRequirementPolicy";
    }
}
