using System;
using System.ComponentModel.DataAnnotations;
using Commons.DomainModel.Base;
using DatabaseAccess;
using DomainModel.Administration;
using DomainModel.Base;
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

            var projectAdmin =
                DbAccess.ReadSingle<ProjectAdministrator, int>(a => a.UserIdentityName == expAdminAdd.UserIdentityName);
            Assert.NotNull(projectAdmin);
            AssertBaseCreated(projectAdmin);
            AssertTablePerHierarchy<ProjectAdministrator, int>(projectAdmin);
            AssertAdministrator(expAdminAdd, projectAdmin);

            var expAdminUpdate = CreateAdministrator<ProjectAdministrator, int>(2);
            ModifyAdministrator<ProjectAdministrator, int>(expAdminAdd, expAdminUpdate);
            var updateResult = DbAccess.Update<ProjectAdministrator, int>(expAdminAdd);
            Assert.True(updateResult.Success);

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
        [Order(3)]
        public void TestDuplicate() {
            var administrator = CreateAdministrator<ProjectAdministrator, int>();
            var addResult = DbAccess.Create<ProjectAdministrator, int>(administrator);
            Assert.True(addResult.Success);

            var duplicate = CreateAdministrator<ProjectAdministrator, int>();
            var duplicateResult = DbAccess.Create<ProjectAdministrator, int>(duplicate);
            Assert.False(duplicateResult.Success);
            Assert.AreEqual(DbResultCode.Duplicate, duplicateResult.ResultCode);
        }

        [Test]
        [Order(11)]
        public void TestCreateAdministratorUserIdentityName() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
            var targetFunction = new Func<ProjectAdministrator, DbResult<ProjectAdministrator>>(DbAccess.Create<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(projectAdmin, "UserIdentityName", 32, targetFunction);
        }

        [Test]
        [Order(12)]
        public void TestCreateAdministratorName() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
            var targetFunction = new Func<ProjectAdministrator, DbResult<ProjectAdministrator>>(DbAccess.Create<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(projectAdmin, "Name", 256, targetFunction);
        }

        [Test]
        [Order(13)]
        public void TestCreateAdministratorEmail() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
            var targetFunction = new Func<ProjectAdministrator, DbResult<ProjectAdministrator>>(DbAccess.Create<ProjectAdministrator, int>);
            ValidateRequiredEmailTests<ProjectAdministrator, int>(projectAdmin, "Email", 1024, targetFunction);
        }

        [Test]
        [Order(14)]
        public void TestCreateAdministratorPhone() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator, int>();
            var targetFunction = new Func<ProjectAdministrator, DbResult<ProjectAdministrator>>(DbAccess.Create<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(projectAdmin, "Phone", 64, targetFunction);
        }

        [Test]
        [Order(21)]
        public void TestUpdateAdministratorUserIdentityName() {
            var administrator = CreateAdministrator<ProjectAdministrator, int>();
            var addResult = DbAccess.Create<ProjectAdministrator, int>(administrator);
            Assert.True(addResult.Success);

            var targetFunction = new Func<ProjectAdministrator, DbResult<ProjectAdministrator>>(DbAccess.Update<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(administrator, "UserIdentityName", 32, targetFunction);
        }

        [Test]
        [Order(22)]
        public void TestUpdateAdministratorName() {
            var administrator = CreateAdministrator<ProjectAdministrator, int>();
            var addResult = DbAccess.Create<ProjectAdministrator, int>(administrator);
            Assert.True(addResult.Success);

            var targetFunction = new Func<ProjectAdministrator, DbResult<ProjectAdministrator>>(DbAccess.Update<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(administrator, "Name", 256, targetFunction);
        }

        [Test]
        [Order(23)]
        public void TestUpdateAdministratorEmail() {
            var administrator = CreateAdministrator<ProjectAdministrator, int>();
            var addResult = DbAccess.Create<ProjectAdministrator, int>(administrator);
            Assert.True(addResult.Success);

            var targetFunction = new Func<ProjectAdministrator, DbResult<ProjectAdministrator>>(DbAccess.Update<ProjectAdministrator, int>);
            ValidateRequiredEmailTests<ProjectAdministrator, int>(administrator, "Email", 1024, targetFunction);
        }

        [Test]
        [Order(24)]
        public void TestUpdateAdministratorPhone() {
            var administrator = CreateAdministrator<ProjectAdministrator, int>();
            var addResult = DbAccess.Create<ProjectAdministrator, int>(administrator);
            Assert.True(addResult.Success);

            var targetFunction = new Func<ProjectAdministrator, DbResult<ProjectAdministrator>>(DbAccess.Update<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(administrator, "Phone", 64, targetFunction);
        }

    }
}