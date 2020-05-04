using DatabaseAccess;
using DomainModel.Administration;
using NUnit.Framework;

namespace UnitTests {
    public class DbExtensionsTest : BaseTest {
        [Test]
        public void TestProjectAdministratorCrud1() {
            var expAdminAdd = CreateAdministrator<ProjectAdministrator, int>();
            var dbResult = DbAccess.Create<ProjectAdministrator, int>(expAdminAdd);
            Assert.True(dbResult.Success);
            Assert.True( 0 != expAdminAdd.Id);

            var projectAdmin = DbAccess.ReadSingle<ProjectAdministrator, int>( a => a.UserIdentityName == expAdminAdd.UserIdentityName);
            Assert.NotNull(projectAdmin);
            AssertTypePerHierarchy<ProjectAdministrator,int>(projectAdmin);
            AssertAdministrator(expAdminAdd, projectAdmin);

            /*
                    ModifyAdministrator<ProjectAdministrator, int>(projectAdmin, expAdminUpdate);
                    Assert.AreEqual(1, DbContext.SaveChanges());

                    var modifiedAdmin = DbContext.ProjectAdministrators
                        .FirstOrDefault(a => a.UserIdentityName == expAdminUpdate.UserIdentityName);
                    Assert.NotNull(modifiedAdmin);
                    AssertBaseModified(modifiedAdmin);
                    AssertAdministrator<ProjectAdministrator, ProjectAdministrator, int>(expAdminUpdate, modifiedAdmin);

                    DbContext.ProjectAdministrators.Remove(modifiedAdmin);
                    Assert.AreEqual(1, DbContext.SaveChanges());

                    var findAdmin = DbContext.Administrators
                        .FirstOrDefault(a => a.UserIdentityName == expAdminUpdate.UserIdentityName);
                    Assert.Null(findAdmin);
                    */
        }

    }
}
