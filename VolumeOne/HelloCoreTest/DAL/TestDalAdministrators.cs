using System;
using System.Linq;

using HelloCoreDal.DomainModel;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

namespace HelloCoreTest.DAL {
    public class TestDalAdministrators : BaseDataLayerTest {
        [Test]
        public void TestChangeType() {
            var expProjectAdminAdd = CreateAdministrator<ProjectAdministrator, int>();
            Assert.True(TryAddEntity<ProjectAdministrator, int>(expProjectAdminAdd));

            var projectAdmin = DbContext.Administrators
                .FirstOrDefault(a => a.UserIdentityName == expProjectAdminAdd.UserIdentityName);
            Assert.NotNull(projectAdmin);
            AssertBaseCreated(projectAdmin);
            AssertTypePerHierarchy<ProjectAdministrator>(projectAdmin);
            AssertAdministrator<ProjectAdministrator, Administrator, int>
                (expProjectAdminAdd, projectAdmin);

            ChangeTypeOfEntry<Administrator, MasterAdministrator, int>(projectAdmin);

            var changedAdmin = DbContext.Administrators
                .FirstOrDefault(a => a.UserIdentityName == expProjectAdminAdd.UserIdentityName);
            Assert.NotNull(changedAdmin);
            Assert.AreEqual(typeof(MasterAdministrator).Name, changedAdmin.Discriminator);
            Assert.AreEqual(typeof(MasterAdministrator), changedAdmin.GetType());
        }

        [Test]
        public void TestProjectAdministratorCrud1() {
            var expAdminAdd = CreateAdministrator<ProjectAdministrator, int>();
            var expAdminUpdate = CreateAdministrator<ProjectAdministrator, int>(2);
            Assert.True(TryAddEntity<ProjectAdministrator, int>(expAdminAdd));

            var projectAdmin = DbContext.ProjectAdministrators
                .FirstOrDefault(a => a.UserIdentityName == expAdminAdd.UserIdentityName);
            Assert.NotNull(projectAdmin);
            AssertBaseCreated(projectAdmin);
            AssertTypePerHierarchy<ProjectAdministrator>(projectAdmin);
            AssertAdministrator<ProjectAdministrator, ProjectAdministrator, int>(expAdminAdd, projectAdmin);

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
        }

        [Test]
        public void TestProjectAdministratorCrud2() {
            var expAdminAdd = CreateAdministrator<ProjectAdministrator, int>();
            var expAdminUpdate = CreateAdministrator<ProjectAdministrator, int>(2);
            Assert.True(TryAddEntity<ProjectAdministrator, int>(expAdminAdd));

            var addedAdmin = DbContext.Administrators
                .FirstOrDefault(a => a.UserIdentityName == expAdminAdd.UserIdentityName);
            Assert.NotNull(addedAdmin);
            AssertBaseCreated(addedAdmin);
            AssertAdministrator<ProjectAdministrator, Administrator, int>(expAdminAdd, addedAdmin);

            ModifyAdministrator<Administrator, int>(addedAdmin, expAdminUpdate);
            Assert.AreEqual(1, DbContext.SaveChanges());

            var modifiedAdmin = DbContext.Administrators
                .FirstOrDefault(a => a.UserIdentityName == expAdminUpdate.UserIdentityName);
            Assert.NotNull(modifiedAdmin);
            Assert.True(modifiedAdmin is ProjectAdministrator);
            AssertBaseModified(modifiedAdmin);
            AssertAdministrator<ProjectAdministrator, Administrator, int>(expAdminUpdate, modifiedAdmin);

            DbContext.Administrators.Remove(modifiedAdmin);
            Assert.AreEqual(1, DbContext.SaveChanges());

            var findAdmin = DbContext.Administrators
                .FirstOrDefault(a => a.UserIdentityName == expAdminUpdate.UserIdentityName);
            Assert.Null(findAdmin);
        }

        [Test]
        public void TestMasterAdministratorCrud1() {
            var expMasterAdminAdd = CreateAdministrator<MasterAdministrator, int>();
            var expMasterAdminUpdate = CreateAdministrator<MasterAdministrator, int>(2);
            Assert.True(TryAddEntity<MasterAdministrator, int>(expMasterAdminAdd));

            var masterAdmin = DbContext.MasterAdministrators
                .FirstOrDefault(a => a.UserIdentityName == expMasterAdminAdd.UserIdentityName);
            Assert.NotNull(masterAdmin);
            AssertBaseCreated(masterAdmin);
            AssertTypePerHierarchy<MasterAdministrator>(masterAdmin);
            AssertAdministrator<MasterAdministrator, MasterAdministrator, int>(expMasterAdminAdd, masterAdmin);

            ModifyAdministrator<MasterAdministrator, int>(masterAdmin, expMasterAdminUpdate);
            Assert.AreEqual(1, DbContext.SaveChanges());

            var modifiedAdmin = DbContext.MasterAdministrators
                .FirstOrDefault(a => a.UserIdentityName == expMasterAdminUpdate.UserIdentityName);
            Assert.NotNull(modifiedAdmin);
            AssertBaseModified(modifiedAdmin);
            AssertAdministrator<MasterAdministrator, MasterAdministrator, int>(expMasterAdminUpdate, modifiedAdmin);

            DbContext.MasterAdministrators.Remove(modifiedAdmin);
            Assert.AreEqual(1, DbContext.SaveChanges());

            var findAdmin = DbContext.Administrators
                .FirstOrDefault(a => a.UserIdentityName == expMasterAdminUpdate.UserIdentityName);
            Assert.Null(findAdmin);
        }

        [Test]
        public void TestMasterAdministratorCrud2() {
            var expMasterAdminAdd = CreateAdministrator<MasterAdministrator, int>();
            var expMasterAdminUpdate = CreateAdministrator<MasterAdministrator, int>(2);
            Assert.True(TryAddEntity<MasterAdministrator, int>(expMasterAdminAdd));

            var masterAdmin = DbContext.Administrators
                .FirstOrDefault(a => a.UserIdentityName == expMasterAdminAdd.UserIdentityName);
            Assert.NotNull(masterAdmin);
            AssertBaseCreated(masterAdmin);
            AssertAdministrator<MasterAdministrator, Administrator, int>(expMasterAdminAdd, masterAdmin);

            ModifyAdministrator<Administrator, int>(masterAdmin, expMasterAdminUpdate);
            Assert.AreEqual(1, DbContext.SaveChanges());

            var modifiedAdmin = DbContext.Administrators
                .FirstOrDefault(a => a.UserIdentityName == expMasterAdminUpdate.UserIdentityName);
            Assert.NotNull(modifiedAdmin);
            Assert.True(modifiedAdmin is MasterAdministrator);
            AssertBaseModified(modifiedAdmin);
            AssertAdministrator<MasterAdministrator, Administrator, int>(expMasterAdminUpdate, modifiedAdmin);

            DbContext.Administrators.Remove(modifiedAdmin);
            Assert.AreEqual(1, DbContext.SaveChanges());

            var findAdmin = DbContext.Administrators
                .FirstOrDefault(a => a.UserIdentityName == expMasterAdminUpdate.UserIdentityName);
            Assert.Null(findAdmin);
        }

        [Test]
        public void TestProjectAdministratorDuplicateIndex1() {
            var projectAdmin1 = CreateAdministrator<ProjectAdministrator, int>();
            TryAddEntity<ProjectAdministrator, int>(projectAdmin1);

            void InvokeException() {
                var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
                TryAddEntity<ProjectAdministrator, int>(projectAdmin);
            }

            Assert.Throws<DbUpdateException>(InvokeException);
        }

        [Test]
        public void TestProjectAdministratorDuplicateIndex2() {
            var projectAdmin1 = CreateAdministrator<ProjectAdministrator, int>();
            TryAddEntity<Administrator, int>(projectAdmin1);

            void InvokeException() {
                var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
                TryAddEntity<Administrator, int>(projectAdmin);
            }

            Assert.Throws<DbUpdateException>(InvokeException);
        }

        [Test]
        public void TestMasterAdministratorDuplicateIndex1() {
            var projectAdmin1 = CreateAdministrator<MasterAdministrator, int>();
            TryAddEntity<MasterAdministrator, int>(projectAdmin1);

            void InvokeException() {
                var projectAdmin = CreateAdministrator<MasterAdministrator, int>();
                TryAddEntity<MasterAdministrator, int>(projectAdmin);
            }

            Assert.Throws<DbUpdateException>(InvokeException);
        }

        [Test]
        public void TestMasterAdministratorDuplicateIndex2() {
            var projectAdmin1 = CreateAdministrator<MasterAdministrator, int>();
            TryAddEntity<Administrator, int>(projectAdmin1);

            void InvokeException() {
                var projectAdmin = CreateAdministrator<MasterAdministrator, int>();
                TryAddEntity<Administrator, int>(projectAdmin);
            }

            Assert.Throws<DbUpdateException>(InvokeException);
        }

        [Test]
        public void TestProjectAdministratorUserIdentityNameAdd() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
            var invokeException =
                new Func<ProjectAdministrator, bool>(TryAddEntity<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(projectAdmin, "UserIdentityName", 32,
                invokeException);
        }

        [Test]
        public void TestProjectAdministratorUserIdentityNameUpdate() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
            Assert.True(TryAddEntity<ProjectAdministrator, int>(projectAdmin));

            var invokeException = new Func<ProjectAdministrator, bool>(TryUpdateEntity<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(projectAdmin, "UserIdentityName", 32,
                invokeException);
        }

        [Test]
        public void TestProjectAdministratorNameAdd() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
            var invokeException =
                new Func<ProjectAdministrator, bool>(TryAddEntity<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(projectAdmin, "Name", 256, invokeException);
        }

        [Test]
        public void TestProjectAdministratorNameUpdate() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
            Assert.True(TryAddEntity<ProjectAdministrator, int>(projectAdmin));

            var invokeException = new Func<ProjectAdministrator, bool>(TryUpdateEntity<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(projectAdmin, "Name", 256, invokeException);
        }

        [Test]
        public void TestProjectAdministratorEmailAdd() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
            var invokeException =
                new Func<ProjectAdministrator, bool>(TryAddEntity<ProjectAdministrator, int>);
            ValidateRequiredEmailTests<ProjectAdministrator, int>(projectAdmin, "Email", 256, invokeException);
        }

        [Test]
        public void TestProjectAdministratorEmailUpdate() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
            Assert.True(TryAddEntity<ProjectAdministrator, int>(projectAdmin));
            var invokeException = new Func<ProjectAdministrator, bool>(TryUpdateEntity<ProjectAdministrator, int>);
            ValidateRequiredEmailTests<ProjectAdministrator, int>(projectAdmin, "Email", 256, invokeException);
        }

        [Test]
        public void TestProjectAdministratorPhoneAdd() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
            var invokeException =
                new Func<ProjectAdministrator, bool>(TryAddEntity<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(projectAdmin, "Phone", 32, invokeException);
        }

        [Test]
        public void TestProjectAdministratorPhoneUpdate() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
            Assert.True(TryAddEntity<ProjectAdministrator, int>(projectAdmin));

            var invokeException = new Func<ProjectAdministrator, bool>(TryUpdateEntity<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(projectAdmin, "Phone", 32, invokeException);
        }
    }
}