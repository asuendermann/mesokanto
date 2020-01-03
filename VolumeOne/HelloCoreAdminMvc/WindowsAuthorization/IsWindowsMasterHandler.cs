using System;
using System.Threading.Tasks;

using HelloCoreBll.BusinesLayerLogic;

using Microsoft.AspNetCore.Authorization;

namespace HelloCoreAdminMvc.WindowsAuthorization {
    public class IsWindowsMasterHandler : AuthorizationHandler<IsWindowsMasterRequirement> {
        private readonly IAdministratorsBllManager adminBllManager;

        public IsWindowsMasterHandler(IAdministratorsBllManager adminBllManager) {
            this.adminBllManager = adminBllManager;
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

            if (adminBllManager.IsMaster(context.User.Identity.Name)) {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}