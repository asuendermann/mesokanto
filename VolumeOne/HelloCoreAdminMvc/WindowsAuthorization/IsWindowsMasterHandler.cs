using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

namespace HelloCoreAdminMvc.WindowsAuthorization {
    public class IsWindowsMasterHandler : AuthorizationHandler<IsWindowsMasterRequirement> {
        private readonly IWindowsAuthorizationService appAuthorizationService;

        public IsWindowsMasterHandler(IWindowsAuthorizationService appAuthorizationService) {
            this.appAuthorizationService = appAuthorizationService;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            IsWindowsMasterRequirement requirement) {
            if (context == null) {
                throw new ArgumentNullException(nameof(context));
            }

            if (requirement == null) {
                throw new ArgumentNullException(nameof(requirement));
            }

            if (appAuthorizationService.IsMaster(context.User.Identity.Name)) {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}