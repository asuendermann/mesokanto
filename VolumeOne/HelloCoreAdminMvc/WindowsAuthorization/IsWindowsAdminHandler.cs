using System;
using System.Threading.Tasks;

using HelloCoreBll.BusinesLayerLogic;

using Microsoft.AspNetCore.Authorization;

namespace HelloCoreAdminMvc.WindowsAuthorization {
    public class IsWindowsAdminHandler : AuthorizationHandler<IsWindowsAdminRequirement> {
        private readonly IAdministratorsBllManager adminBllManager;

        public IsWindowsAdminHandler(IAdministratorsBllManager adminBllManager) {
            this.adminBllManager = adminBllManager;
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

            if (adminBllManager.IsAdmin(context.User.Identity.Name)) {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}