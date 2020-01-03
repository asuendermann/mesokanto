using System.Linq;

using HelloCoreBll.BusinesLayerLogic;

using HelloCoreDal.DomainModel;

using HelloCoreTest.DAL;

using NUnit.Framework;

namespace HelloCoreTest.BL {
    public class TestBlAdministrators : BaseDataLayerTest {
        private readonly IAdministratorsBllManager adminBllMgr;


        public TestBlAdministrators() {
            adminBllMgr = ServiceProvider.GetService(typeof(IAdministratorsBllManager))
                as IAdministratorsBllManager;
        }

        [SetUp]
        public override void Setup() {
            base.Setup();
            Assert.NotNull(adminBllMgr);
        }

        [Test]
        public void TestReadMasterAdministrators() {
            var expMas = PopulateAdministrators<MasterAdministrator>(3);
            var expPas = PopulateAdministrators<ProjectAdministrator>(4);
            var masters = adminBllMgr.ReadMasterAdministrators();
            Assert.AreEqual(3, masters.Count());
        }

        [Test]
        public void TestCountAdministrators() {
            var expMas = PopulateAdministrators<MasterAdministrator>(3);
            var expPas = PopulateAdministrators<ProjectAdministrator>(4);

            var adminCount = adminBllMgr.CountAdministrators();
            Assert.AreEqual(7, adminCount);

            var masterCount = adminBllMgr.CountMasterAdministrators();
            Assert.AreEqual(3, masterCount);

            var projectCount = adminBllMgr.CountProjectAdministrators();
            Assert.AreEqual(4, projectCount);
        }
        [Test]
        public void TestIsAdmin() {
            var expMas = PopulateAdministrators<MasterAdministrator>(1);
            var expPas = PopulateAdministrators<ProjectAdministrator>(1);

            var isAdmin1 = adminBllMgr.IsAdmin(expMas[0].UserIdentityName);
            Assert.True(isAdmin1);
            var isAdmin2 = adminBllMgr.IsAdmin(expMas[0].UserIdentityName);
            Assert.True(isAdmin2);


        }
        [Test]
        public void TestIsMaster() {
            var expMas = PopulateAdministrators<MasterAdministrator>(1);
            var expPas = PopulateAdministrators<ProjectAdministrator>(1);

            var isMaster1 = adminBllMgr.IsMaster(expMas[0].UserIdentityName);
            Assert.True(isMaster1);
            var isMaster2 = adminBllMgr.IsMaster(expPas[0].UserIdentityName);
            Assert.False(isMaster2);


        }
    }
}