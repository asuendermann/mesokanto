using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

namespace HelloCoreAdminMvc.WindowsAuthorization {
    public class IsWindowsAdminHandler : AuthorizationHandler<IsWindowsAdminRequirement> {
        private readonly IWindowsAuthorizationService appAuthorizationService;

        public IsWindowsAdminHandler(IWindowsAuthorizationService appAuthorizationService) {
            this.appAuthorizationService = appAuthorizationService;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            IsWindowsAdminRequirement requirement) {
            if (context == null) {
                throw new ArgumentNullException(nameof(context));
            }

            if (requirement == null) {
                throw new ArgumentNullException(nameof(requirement));
            }

            if (appAuthorizationService.IsAdmin(context.User.Identity.Name)) {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}