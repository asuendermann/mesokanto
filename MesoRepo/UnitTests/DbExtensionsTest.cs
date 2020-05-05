using System;
using System.Linq;
using DatabaseAccess;
using DomainModel.Administration;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace UnitTests {
    public class DbExtensionsTest : BaseTest {
        [Test]
        [Order(1)]
        public void TestLifeCycle() {
            var expAdminAdd = CreateAdministrator<ProjectAdministrator, int>();
            var addResult = DbAccess.Create<ProjectAdministrator, int>(expAdminAdd);
            Assert.True(addResult.Success);
            Assert.True(0 != expAdminAdd.Id);
            Assert.AreEqual(EntityState.Unchanged, DbAccess.Entry(expAdminAdd).State);

            var projectAdmin =
                DbAccess.ReadSingle<ProjectAdministrator, int>(a => a.UserIdentityName == expAdminAdd.UserIdentityName);
            Assert.NotNull(projectAdmin);
            AssertBaseCreated(projectAdmin);
            AssertTablePerHierarchy<ProjectAdministrator, int>(projectAdmin);
            AssertAdministrator(expAdminAdd, projectAdmin);

            var expAdminUpdate = CreateAdministrator<ProjectAdministrator, int>(2);
            ModifyAdministrator<ProjectAdministrator, int>(expAdminAdd, expAdminUpdate);
            Assert.AreEqual(EntityState.Modified, DbAccess.Entry(expAdminAdd).State);
            var updateResult = DbAccess.Update<ProjectAdministrator, int>(expAdminAdd);
            Assert.True(updateResult.Success);
            Assert.AreEqual(EntityState.Unchanged, DbAccess.Entry(expAdminAdd).State);

            var modifiedAdmin =
                DbAccess.ReadSingle<ProjectAdministrator, int>(a =>
                    a.UserIdentityName == expAdminUpdate.UserIdentityName);
            AssertBaseModified(modifiedAdmin);
            AssertAdministrator(expAdminUpdate, modifiedAdmin);

            var deleteResult = DbAccess.Delete<ProjectAdministrator, int>(expAdminAdd);
            Assert.True(deleteResult.Success);

            var deletedAdmin =
                DbAccess.ReadSingle<ProjectAdministrator, int>(a =>
                    a.UserIdentityName == expAdminUpdate.UserIdentityName);
            Assert.Null(deletedAdmin);
        }

        [Test]
        [Order(2)]
        public void TestLifeCycleMany() {
            var expAdministrators = CreateEntities(CreateAdministrator<ProjectAdministrator, int>, 3);
            var createResults = DbAccess.Create<ProjectAdministrator, int>(expAdministrators);
            Assert.True(createResults.Success);

            var administrators = DbAccess.Read<ProjectAdministrator, int>();
            Assert.AreEqual(expAdministrators.Count(), administrators.Count());
            foreach (var expAdministrator in expAdministrators) {
                Assert.True(administrators.Any(a => a.UserIdentityName == expAdministrator.UserIdentityName));
            }

            var deleteResult = DbAccess.Delete<ProjectAdministrator, int>(expAdministrators);
            Assert.True(deleteResult.Success);

            var deletedAdministrators = DbAccess.Read<ProjectAdministrator, int>();
            Assert.False(deletedAdministrators.Any());
        }

        [Test]
        [Order(3)]
        public void TestChangeType() {
            var projectAdministrator = CreateAdministrator<ProjectAdministrator, int>();
            var addResult = DbAccess.Create<ProjectAdministrator, int>(projectAdministrator);
            Assert.True(addResult.Success);

            ChangeTypeOfEntry<Administrator, MasterAdministrator, int>(projectAdministrator);

            var masterAdministrator = DbAccess.ReadSingle<Administrator, int>
                (a => a.UserIdentityName == projectAdministrator.UserIdentityName);
            Assert.NotNull(masterAdministrator);
            Assert.AreEqual(nameof(MasterAdministrator), masterAdministrator.Discriminator);
            Assert.AreEqual(typeof(MasterAdministrator), masterAdministrator.GetType());
        }

        [Test]
        [Order(4)]
        public void TestLifeCycleId() {
            var administrator1 = CreateAdministrator<ProjectAdministrator, int>();
            administrator1.Id = 1;
            var createResult1 = DbAccess.Create<ProjectAdministrator, int>(administrator1);
            Assert.False(createResult1.Success);
            Assert.AreEqual(DbResultCode.Impractical, createResult1.ResultCode);
            Assert.AreEqual(EntityState.Detached, DbAccess.Entry(administrator1).State);

            var administrator2 = CreateAdministrator<ProjectAdministrator, int>(2);
            var updateResult = DbAccess.Update<ProjectAdministrator, int>(administrator2);
            Assert.False(updateResult.Success);
            Assert.AreEqual(DbResultCode.Impractical, updateResult.ResultCode);
            Assert.AreEqual(EntityState.Detached, DbAccess.Entry(administrator2).State);

            var administrator3 = CreateAdministrator<ProjectAdministrator, int>(3);
            var deleteResult = DbAccess.Delete<ProjectAdministrator, int>(administrator3);
            Assert.False(deleteResult.Success);
            Assert.AreEqual(DbResultCode.Impractical, deleteResult.ResultCode);
            Assert.AreEqual(EntityState.Detached, DbAccess.Entry(administrator3).State);
        }

        [Test]
        [Order(5)]
        public void TestLifeCycleDuplicate() {
            var administrator1 = CreateAdministrator<ProjectAdministrator, int>();
            var createResult1 = DbAccess.Create<ProjectAdministrator, int>(administrator1);
            Assert.True(createResult1.Success);

            var duplicateAdministrator = CreateAdministrator<ProjectAdministrator, int>();
            var duplicateResult = DbAccess.Create<ProjectAdministrator, int>(duplicateAdministrator);
            Assert.False(duplicateResult.Success);
            Assert.AreEqual(DbResultCode.Duplicate, duplicateResult.ResultCode);

            var administrator2 = CreateAdministrator<ProjectAdministrator, int>(2);
            var createResult2 = DbAccess.Create<ProjectAdministrator, int>(administrator2);
            Assert.True(createResult2.Success);

            ModifyAdministrator<ProjectAdministrator, int>(administrator2, administrator1);
            var updateResult = DbAccess.Update<ProjectAdministrator, int>(administrator2);
            Assert.False(updateResult.Success);
            Assert.AreEqual(DbResultCode.Duplicate, updateResult.ResultCode);
        }

        [Test]
        [Order(11)]
        public void TestCreateAdministratorUserIdentityName() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
            var targetFunction =
                new Func<ProjectAdministrator, DbResult<ProjectAdministrator>>(DbAccess
                    .Create<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(projectAdmin, "UserIdentityName", 32,
                targetFunction);
        }

        [Test]
        [Order(12)]
        public void TestCreateAdministratorName() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
            var targetFunction =
                new Func<ProjectAdministrator, DbResult<ProjectAdministrator>>(DbAccess
                    .Create<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(projectAdmin, "Name", 256, targetFunction);
        }

        [Test]
        [Order(13)]
        public void TestCreateAdministratorEmail() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
            var targetFunction =
                new Func<ProjectAdministrator, DbResult<ProjectAdministrator>>(DbAccess
                    .Create<ProjectAdministrator, int>);
            ValidateRequiredEmailTests<ProjectAdministrator, int>(projectAdmin, "Email", 1024, targetFunction);
        }

        [Test]
        [Order(14)]
        public void TestCreateAdministratorPhone() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
            var targetFunction =
                new Func<ProjectAdministrator, DbResult<ProjectAdministrator>>(DbAccess
                    .Create<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(projectAdmin, "Phone", 64, targetFunction);
        }

        [Test]
        [Order(21)]
        public void TestUpdateAdministratorUserIdentityName() {
            var administrator = CreateAdministrator<ProjectAdministrator, int>();
            var addResult = DbAccess.Create<ProjectAdministrator, int>(administrator);
            Assert.True(addResult.Success);

            var targetFunction =
                new Func<ProjectAdministrator, DbResult<ProjectAdministrator>>(DbAccess
                    .Update<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(administrator, "UserIdentityName", 32,
                targetFunction);
        }

        [Test]
        [Order(22)]
        public void TestUpdateAdministratorName() {
            var administrator = CreateAdministrator<ProjectAdministrator, int>();
            var addResult = DbAccess.Create<ProjectAdministrator, int>(administrator);
            Assert.True(addResult.Success);

            var targetFunction =
                new Func<ProjectAdministrator, DbResult<ProjectAdministrator>>(DbAccess
                    .Update<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(administrator, "Name", 256, targetFunction);
        }

        [Test]
        [Order(23)]
        public void TestUpdateAdministratorEmail() {
            var administrator = CreateAdministrator<ProjectAdministrator, int>();
            var addResult = DbAccess.Create<ProjectAdministrator, int>(administrator);
            Assert.True(addResult.Success);

            var targetFunction =
                new Func<ProjectAdministrator, DbResult<ProjectAdministrator>>(DbAccess
                    .Update<ProjectAdministrator, int>);
            ValidateRequiredEmailTests<ProjectAdministrator, int>(administrator, "Email", 1024, targetFunction);
        }

        [Test]
        [Order(24)]
        public void TestUpdateAdministratorPhone() {
            var administrator = CreateAdministrator<ProjectAdministrator, int>();
            var addResult = DbAccess.Create<ProjectAdministrator, int>(administrator);
            Assert.True(addResult.Success);

            var targetFunction =
                new Func<ProjectAdministrator, DbResult<ProjectAdministrator>>(DbAccess
                    .Update<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(administrator, "Phone", 64, targetFunction);
        }
    }
}