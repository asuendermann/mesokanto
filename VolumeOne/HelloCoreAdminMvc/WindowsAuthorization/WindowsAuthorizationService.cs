using System;

using HelloCoreBll.BusinesLayerLogic;

namespace HelloCoreAdminMvc.WindowsAuthorization {
    public class WindowsAuthorizationService : IWindowsAuthorizationService {
        private readonly IAdministratorsBllManager adminBllManager;

        public WindowsAuthorizationService(IServiceProvider serviceProvider) {
            adminBllManager =
                serviceProvider.GetService(typeof(IAdministratorsBllManager)) as IAdministratorsBllManager;
        }

        public bool IsAdmin(string userIdentityName) {
            return adminBllManager.IsAdmin(userIdentityName);
        }

        public bool IsMaster(string userIdentityName) {
            return adminBllManager.IsMaster(userIdentityName);
        }
    }
}