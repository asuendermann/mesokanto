using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseAccess;
using DatabaseAccess.SortFilters;
using DomainModel.Administration;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace UnitTests {
    public class DbExtensionsTest : BaseTest {
        [Test]
        [Order(1)]
        public void TestLifeCycle() {
            var expAdminAdd = CreateAdministrator<ProjectAdministrator>();
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

            var expAdminUpdate = CreateAdministrator<ProjectAdministrator>(2);
            ModifyAdministrator(expAdminAdd, expAdminUpdate);
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
            var expAdministrators = CreateEntities(CreateAdministrator<ProjectAdministrator>, 3);
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
            var projectAdministrator = CreateAdministrator<ProjectAdministrator>();
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
            var administrator1 = CreateAdministrator<ProjectAdministrator>();
            administrator1.Id = 1;
            var createResult1 = DbAccess.Create<ProjectAdministrator, int>(administrator1);
            Assert.False(createResult1.Success);
            Assert.AreEqual(DbResultCode.Impractical, createResult1.ResultCode);
            Assert.AreEqual(EntityState.Detached, DbAccess.Entry(administrator1).State);

            var administrator2 = CreateAdministrator<ProjectAdministrator>(2);
            var updateResult = DbAccess.Update<ProjectAdministrator, int>(administrator2);
            Assert.False(updateResult.Success);
            Assert.AreEqual(DbResultCode.Impractical, updateResult.ResultCode);
            Assert.AreEqual(EntityState.Detached, DbAccess.Entry(administrator2).State);

            var administrator3 = CreateAdministrator<ProjectAdministrator>(3);
            var deleteResult = DbAccess.Delete<ProjectAdministrator, int>(administrator3);
            Assert.False(deleteResult.Success);
            Assert.AreEqual(DbResultCode.Impractical, deleteResult.ResultCode);
            Assert.AreEqual(EntityState.Detached, DbAccess.Entry(administrator3).State);
        }

        [Test]
        [Order(5)]
        public void TestLifeCycleDuplicate() {
            var administrator1 = CreateAdministrator<ProjectAdministrator>();
            var createResult1 = DbAccess.Create<ProjectAdministrator, int>(administrator1);
            Assert.True(createResult1.Success);

            var duplicateAdministrator = CreateAdministrator<ProjectAdministrator>();
            var duplicateResult = DbAccess.Create<ProjectAdministrator, int>(duplicateAdministrator);
            Assert.False(duplicateResult.Success);
            Assert.AreEqual(DbResultCode.Duplicate, duplicateResult.ResultCode);

            var administrator2 = CreateAdministrator<ProjectAdministrator>(2);
            var createResult2 = DbAccess.Create<ProjectAdministrator, int>(administrator2);
            Assert.True(createResult2.Success);

            ModifyAdministrator(administrator2, administrator1);
            var updateResult = DbAccess.Update<ProjectAdministrator, int>(administrator2);
            Assert.False(updateResult.Success);
            Assert.AreEqual(DbResultCode.Duplicate, updateResult.ResultCode);
        }

        [Test]
        [Order(6)]
        public void TestPagedResult() {
            var random = new Random(DateTime.Now.GetHashCode());
            var administrators = new List<ProjectAdministrator>();
            while (administrators.Count < 25) {
                var index = random.Next(1000);
                var administrator = CreateAdministrator<ProjectAdministrator>(index);
                if (administrators.All(a => a.UserIdentityName != administrator.UserIdentityName)) {
                    administrators.Add(administrator);
                }
            }

            var createResults = DbAccess.Create<ProjectAdministrator, int>(administrators);
            Assert.True(createResults.Success);
            var expAdministrators = administrators.OrderByDescending(a => a.Id).ToList();
            var expRowCount = administrators.Count;
            var expPageCount = expRowCount / PagedResult<ProjectAdministrator>.PageSize_10;
            if (0 < expRowCount % PagedResult<ProjectAdministrator>.PageSize_10) {
                expPageCount++;
            }

            for (var pageNumber = 1; pageNumber <= expPageCount; pageNumber++) {
                Console.WriteLine($"Page {pageNumber}");
                var pagedResult = new PagedResult<ProjectAdministrator> {
                    PageNumber = pageNumber
                };
                DbAccess.ReadPage<ProjectAdministrator, int>(pagedResult);

                Assert.AreEqual(pageNumber, pagedResult.PageNumber);
                Assert.AreEqual(expRowCount, pagedResult.RowCount);

                Assert.AreEqual(expPageCount, pagedResult.PageCount);

                var expPageSize = pageNumber < expPageCount || 0 == expPageCount % pagedResult.PageSize
                    ? pagedResult.PageSize
                    : expRowCount % pagedResult.PageSize;
                Assert.AreEqual(expPageSize, pagedResult.Results.Count());
                for (var i = 0; i < expPageSize; i++) {
                    var index = (pagedResult.PageNumber - 1) * pagedResult.PageSize + i;
                    Console.WriteLine($"Id {pagedResult.Results[i].Id} {pagedResult.Results[i].UserIdentityName}");
                    AssertAdministrator(expAdministrators[index], pagedResult.Results[i]);
                }
            }
        }

        [Test]
        [Order(7)]
        public void TestPagedResultSorted() {
            var random = new Random(DateTime.Now.GetHashCode());
            var administrators = new List<ProjectAdministrator>();
            while (administrators.Count < 25) {
                var index = random.Next(1000);
                var administrator = CreateAdministrator<ProjectAdministrator>(index);
                if (administrators.All(a => a.UserIdentityName != administrator.UserIdentityName)) {
                    administrators.Add(administrator);
                }
            }

            var createResults = DbAccess.Create<ProjectAdministrator, int>(administrators);
            Assert.True(createResults.Success);
            var expAdministrators = administrators.OrderBy(a => a.UserIdentityName).ToList();
            var sortFilter = new SortFilterString<ProjectAdministrator> {
                Expression = p => p.UserIdentityName
            };
            var expRowCount = administrators.Count;
            var expPageCount = expRowCount / PagedResult<ProjectAdministrator>.PageSize_10;
            if (0 < expRowCount % PagedResult<ProjectAdministrator>.PageSize_10) {
                expPageCount++;
            }

            for (var pageNumber = 1; pageNumber <= expPageCount; pageNumber++) {
                Console.WriteLine($"Page {pageNumber}");
                var pagedResult = new PagedResult<ProjectAdministrator> {
                    SortFilters = new List<SortFilter<ProjectAdministrator>> {sortFilter},
                    PageNumber = pageNumber
                };
                DbAccess.ReadPage<ProjectAdministrator, int>(pagedResult);

                Assert.AreEqual(pageNumber, pagedResult.PageNumber);
                Assert.AreEqual(expRowCount, pagedResult.RowCount);

                Assert.AreEqual(expPageCount, pagedResult.PageCount);

                var expPageSize = pageNumber < expPageCount || 0 == expPageCount % pagedResult.PageSize
                    ? pagedResult.PageSize
                    : expRowCount % pagedResult.PageSize;
                Assert.AreEqual(expPageSize, pagedResult.Results.Count());
                for (var i = 0; i < expPageSize; i++) {
                    var index = (pagedResult.PageNumber - 1) * pagedResult.PageSize + i;
                    Console.WriteLine(pagedResult.Results[i].UserIdentityName);
                    AssertAdministrator(expAdministrators[index], pagedResult.Results[i]);
                }
            }

            sortFilter.Descending = true;
            expAdministrators = administrators.OrderByDescending(a => a.UserIdentityName).ToList();
            for (var pageNumber = 1; pageNumber <= expPageCount; pageNumber++) {
                Console.WriteLine($"Page {pageNumber}");
                var pagedResult = new PagedResult<ProjectAdministrator> {
                    SortFilters = new List<SortFilter<ProjectAdministrator>> {sortFilter},
                    PageNumber = pageNumber
                };
                DbAccess.ReadPage<ProjectAdministrator, int>(pagedResult);
                var expPageSize = pageNumber < expPageCount || 0 == expPageCount % pagedResult.PageSize
                    ? pagedResult.PageSize
                    : expRowCount % pagedResult.PageSize;
                for (var i = 0; i < expPageSize; i++) {
                    var index = (pagedResult.PageNumber - 1) * pagedResult.PageSize + i;
                    Console.WriteLine(pagedResult.Results[i].UserIdentityName);
                    AssertAdministrator(expAdministrators[index], pagedResult.Results[i]);
                }
            }
        }

        [Test]
        [Order(8)]
        public void TestPagedResultFiltered() {
            var administrators = CreateEntities(CreateAdministrator<ProjectAdministrator>, 100);

            var createResults = DbAccess.Create<ProjectAdministrator, int>(administrators);
            Assert.True(createResults.Success);
            var expAdministrators = administrators
                .Where(a => a.UserIdentityName.Contains("9"))
                .OrderByDescending(a => a.Id).ToList();

            var expRowCount = expAdministrators.Count;
            var expPageCount = expRowCount / PagedResult<ProjectAdministrator>.PageSize_05;
            if (0 < expRowCount % PagedResult<ProjectAdministrator>.PageSize_05) {
                expPageCount++;
            }

            for (var pageNumber = 1; pageNumber <= expPageCount; pageNumber++) {
                Console.WriteLine($"Page {pageNumber}");
                var pagedResult = new PagedResult<ProjectAdministrator> {
                    PageSize = PagedResult<ProjectAdministrator>.PageSize_05,
                    Filter = a => a.UserIdentityName.Contains("9"),
                    PageNumber = pageNumber
                };
                DbAccess.ReadPage<ProjectAdministrator, int>(pagedResult);

                Assert.AreEqual(pageNumber, pagedResult.PageNumber);
                Assert.AreEqual(expRowCount, pagedResult.RowCount);

                Assert.AreEqual(expPageCount, pagedResult.PageCount);

                var expPageSize = pageNumber < expPageCount || 0 == expPageCount % pagedResult.PageSize
                    ? pagedResult.PageSize
                    : expRowCount % pagedResult.PageSize;
                Assert.AreEqual(expPageSize, pagedResult.Results.Count());
                for (var i = 0; i < expPageSize; i++) {
                    var index = (pagedResult.PageNumber - 1) * pagedResult.PageSize + i;
                    Console.WriteLine(pagedResult.Results[i].UserIdentityName);
                    AssertAdministrator(expAdministrators[index], pagedResult.Results[i]);
                }
            }
        }


        [Test]
        [Order(11)]
        public void TestCreateAdministratorUserIdentityName() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator>();
            var targetFunction =
                new Func<ProjectAdministrator, DbResult<ProjectAdministrator>>(DbAccess
                    .Create<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(projectAdmin, "UserIdentityName", 32,
                targetFunction);
        }

        [Test]
        [Order(12)]
        public void TestCreateAdministratorName() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator>();
            var targetFunction =
                new Func<ProjectAdministrator, DbResult<ProjectAdministrator>>(DbAccess
                    .Create<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(projectAdmin, "Name", 256, targetFunction);
        }

        [Test]
        [Order(13)]
        public void TestCreateAdministratorEmail() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator>();
            var targetFunction =
                new Func<ProjectAdministrator, DbResult<ProjectAdministrator>>(DbAccess
                    .Create<ProjectAdministrator, int>);
            ValidateRequiredEmailTests<ProjectAdministrator, int>(projectAdmin, "Email", 1024, targetFunction);
        }

        [Test]
        [Order(14)]
        public void TestCreateAdministratorPhone() {
            var projectAdmin = CreateAdministrator<ProjectAdministrator>();
            var targetFunction =
                new Func<ProjectAdministrator, DbResult<ProjectAdministrator>>(DbAccess
                    .Create<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(projectAdmin, "Phone", 64, targetFunction);
        }

        [Test]
        [Order(21)]
        public void TestUpdateAdministratorUserIdentityName() {
            var administrator = CreateAdministrator<ProjectAdministrator>();
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
            var administrator = CreateAdministrator<ProjectAdministrator>();
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
            var administrator = CreateAdministrator<ProjectAdministrator>();
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
            var administrator = CreateAdministrator<ProjectAdministrator>();
            var addResult = DbAccess.Create<ProjectAdministrator, int>(administrator);
            Assert.True(addResult.Success);

            var targetFunction =
                new Func<ProjectAdministrator, DbResult<ProjectAdministrator>>(DbAccess
                    .Update<ProjectAdministrator, int>);
            ValidateRequiredStringTests<ProjectAdministrator, int>(administrator, "Phone", 64, targetFunction);
        }
    }
}