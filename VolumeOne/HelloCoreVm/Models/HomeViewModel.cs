using System;

using HelloCoreBll.DataTransferObjects;

using HelloCoreVm.Base;

namespace HelloCoreVm.Models {
    public class HomeViewModel : ListViewModel<MasterAdministratorDto, int> {

        public int AdministratorCount { get; set; }

        public int MasterAdministratorCount { get; set; }
    
        public int ProjectAdministratorCount { get; set; }
    }
}