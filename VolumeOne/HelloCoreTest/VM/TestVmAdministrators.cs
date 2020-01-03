using System.Linq;

using HelloCoreDal.DomainModel;

using HelloCoreVm;

using NUnit.Framework;

namespace HelloCoreTest.VM {
    public class TestVmAdministrators : BaseDataLayerTest {
        [Test]
        public void TestHomeVm() {
            var expMas = PopulateAdministrators<MasterAdministrator>(3);
            var expPas = PopulateAdministrators<ProjectAdministrator>(4);
            var vmAdminManager = (IViewModelManager) ServiceProvider.GetService(typeof(IViewModelManager)) ;
            Assert.NotNull(vmAdminManager);
            var homeVm = vmAdminManager.GetHomeViewModel();
            Assert.AreEqual(3, homeVm.Auditables.Count());
            Assert.AreEqual(3, homeVm.MasterAdministratorCount);
            Assert.AreEqual(4, homeVm.ProjectAdministratorCount);
            Assert.AreEqual(7, homeVm.AdministratorCount);
        }
    }
}