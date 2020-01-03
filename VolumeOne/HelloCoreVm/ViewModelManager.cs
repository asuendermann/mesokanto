using System;

using HelloCoreBll.BusinesLayerLogic;

using HelloCoreVm.Models;

namespace HelloCoreVm {
    public class ViewModelManager : IViewModelManager {
        private readonly IAdministratorsBllManager adminBllManager;

        public ViewModelManager(IServiceProvider serviceProvider) {
            if (null == serviceProvider) {
                throw new ArgumentException(nameof(serviceProvider));
            }

            adminBllManager =
                serviceProvider.GetService(typeof(IAdministratorsBllManager)) as IAdministratorsBllManager;
        }

        public HomeViewModel GetHomeViewModel() {
            var masters = adminBllManager.ReadMasterAdministrators();
            var model = new HomeViewModel {
                Auditables = masters,
                AdministratorCount = adminBllManager.CountAdministrators(),
                MasterAdministratorCount = adminBllManager.CountMasterAdministrators(),
                ProjectAdministratorCount = adminBllManager.CountProjectAdministrators()
            };
            return model;
        }
    }
}