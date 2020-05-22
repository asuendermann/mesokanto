using System;
using System.Collections.Generic;
using System.Linq;
using Commons.DomainModel;
using DatabaseAccess;
using DatabaseAccess.SortFilters;
using DomainModel.Domain;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace UnitTests {
    public class DbExtensionsTest : BaseTest {
        [Test]
        [Order(1)]
        public void TestLifeCycle() {
            var expTeamMember = CreateTeamMember<TeamMember>();
            var createResult = DbAccess.Create<TeamMember, int>(expTeamMember);
            Assert.True(createResult.Success);
            Assert.True(0 != expTeamMember.Id);
            Assert.AreEqual(EntityState.Unchanged, DbAccess.Entry(expTeamMember).State);

            var teamMember = DbAccess.ReadSingle<TeamMember, int>
                (a => a.UserId == expTeamMember.UserId);
            Assert.NotNull(teamMember);
            AssertBaseCreated(teamMember);
            AssertTablePerHierarchy<TeamMember, int>(teamMember);
            AssertTeamMember(expTeamMember, teamMember);

            var expTeamMemberUpdate = CreateTeamMember<TeamMember>(2);
            ModifyTeamMember(expTeamMember, expTeamMemberUpdate);
            Assert.AreEqual(EntityState.Modified, DbAccess.Entry(expTeamMember).State);
            var updateResult = DbAccess.Update<TeamMember, int>(expTeamMember);
            Assert.True(updateResult.Success);
            Assert.AreEqual(EntityState.Unchanged, DbAccess.Entry(expTeamMember).State);

            var modifiedTeamMember = DbAccess.ReadSingle<TeamMember, int>
                (a => a.UserId == expTeamMemberUpdate.UserId);
            AssertBaseModified(modifiedTeamMember);
            AssertTeamMember(expTeamMemberUpdate, modifiedTeamMember);

            var deleteResult = DbAccess.Delete<TeamMember, int>(expTeamMember);
            Assert.True(deleteResult.Success);

            var deletedAdmin =
                DbAccess.ReadSingle<TeamMember, int>(a =>
                    a.UserId == expTeamMemberUpdate.UserId);
            Assert.Null(deletedAdmin);
        }

        /// <summary>
        ///     https://stackoverflow.com/questions/10822656/entity-framework-include-multiple-levels-of-properties
        /// </summary>
        [Test]
        [Order(2)]
        public void TestLifeCycleWithChildren() {
            var expProject = CreateProject<Project>();
            var teamMember = CreateTeamMember<TeamMember>();
            expProject.ProjectTeamMembers = new List<ProjectTeamMember> {
                new ProjectTeamMember {
                    TeamMember = teamMember
                }
            };
            var addResult = DbAccess.CreateWithChildren<Project, int>(expProject);
            Assert.True(addResult.Success);

            var project = DbAccess.Set<Project>().Where(p=>p.Identifier == expProject.Identifier)
                .Include(p=>p.ProjectTeamMembers).ThenInclude(p=>p.TeamMember)
                .FirstOrDefault();

            AssertProject(expProject, project);
        }

        [Test]
        [Order(3)]
        public void TestLifeCycleMany() {
            var expTeamMembers = CreateEntities(CreateTeamMember<TeamMember>, 3);
            var createResults = DbAccess.Create<TeamMember, int>(expTeamMembers);
            Assert.True(createResults.Success);

            var teamMembers = DbAccess.Read<TeamMember, int>();
            Assert.AreEqual(expTeamMembers.Count(), teamMembers.Count());
            foreach (var expAdministrator in expTeamMembers) {
                Assert.True(teamMembers.Any(a => a.UserId == expAdministrator.UserId));
            }

            var deleteResult = DbAccess.Delete<TeamMember, int>(expTeamMembers);
            Assert.True(deleteResult.Success);

            var deletedAdministrators = DbAccess.Read<TeamMember, int>();
            Assert.False(deletedAdministrators.Any());
        }

        [Test]
        [Order(4)]
        public void TestChangeType() {
            var expTeamMember = CreateTeamMember<TeamMember>();
            var addResult = DbAccess.Create<TeamMember, int>(expTeamMember);
            Assert.True(addResult.Success);

            ChangeTypeOfEntry<TeamMember, ScrumMaster, int>(expTeamMember);

            var masterAdministrator = DbAccess.ReadSingle<TeamMember, int>
                (a => a.UserId == expTeamMember.UserId);
            Assert.NotNull(masterAdministrator);
            Assert.AreEqual(nameof(ScrumMaster), masterAdministrator.Discriminator);
            Assert.AreEqual(typeof(ScrumMaster), masterAdministrator.GetType());
        }

        [Test]
        [Order(5)]
        public void TestLifeCycleId() {
            var teamMember1 = CreateTeamMember<TeamMember>();
            teamMember1.Id = 1;
            var createResult1 = DbAccess.Create<TeamMember, int>(teamMember1);
            Assert.False(createResult1.Success);
            Assert.AreEqual(DbResultCode.Impractical, createResult1.ResultCode);
            Assert.AreEqual(EntityState.Detached, DbAccess.Entry(teamMember1).State);

            var teamMember2 = CreateTeamMember<TeamMember>(2);
            var updateResult = DbAccess.Update<TeamMember, int>(teamMember2);
            Assert.False(updateResult.Success);
            Assert.AreEqual(DbResultCode.Impractical, updateResult.ResultCode);
            Assert.AreEqual(EntityState.Detached, DbAccess.Entry(teamMember2).State);

            var teamMember3 = CreateTeamMember<TeamMember>(3);
            var deleteResult = DbAccess.Delete<TeamMember, int>(teamMember3);
            Assert.False(deleteResult.Success);
            Assert.AreEqual(DbResultCode.Impractical, deleteResult.ResultCode);
            Assert.AreEqual(EntityState.Detached, DbAccess.Entry(teamMember3).State);
        }

        [Test]
        [Order(6)]
        public void TestLifeCycleDuplicate() {
            var teamMember1 = CreateTeamMember<TeamMember>();
            var createResult1 = DbAccess.Create<TeamMember, int>(teamMember1);
            Assert.True(createResult1.Success);

            var duplicateTeamMember = CreateTeamMember<TeamMember>();
            var duplicateResult = DbAccess.Create<TeamMember, int>(duplicateTeamMember);
            Assert.False(duplicateResult.Success);
            Assert.AreEqual(DbResultCode.Duplicate, duplicateResult.ResultCode);

            var teamMember2 = CreateTeamMember<TeamMember>(2);
            var createResult2 = DbAccess.Create<TeamMember, int>(teamMember2);
            Assert.True(createResult2.Success);

            ModifyTeamMember(teamMember2, teamMember1);
            var updateResult = DbAccess.Update<TeamMember, int>(teamMember2);
            Assert.False(updateResult.Success);
            Assert.AreEqual(DbResultCode.Duplicate, updateResult.ResultCode);
        }

        [Test]
        [Order(7)]
        public void TestPagedResult() {
            var random = new Random(DateTime.Now.GetHashCode());
            var teamMembers = new List<TeamMember>();
            while (teamMembers.Count < 25) {
                var index = random.Next(1000);
                var teamMember = CreateTeamMember<TeamMember>(index);
                if (teamMembers.All(a => a.UserId != teamMember.UserId)) {
                    teamMembers.Add(teamMember);
                }
            }

            var createResults = DbAccess.Create<TeamMember, int>(teamMembers);
            Assert.True(createResults.Success);
            var expTeamMembers = teamMembers.OrderByDescending(a => a.Id).ToList();
            var expRowCount = teamMembers.Count;
            var expPageCount = expRowCount / PagedResult<TeamMember>.PageSize_10;
            if (0 < expRowCount % PagedResult<TeamMember>.PageSize_10) {
                expPageCount++;
            }

            for (var pageNumber = 1; pageNumber <= expPageCount; pageNumber++) {
                Console.WriteLine($"Page {pageNumber}");
                var pagedResult = new PagedResult<TeamMember> {
                    PageNumber = pageNumber
                };
                DbAccess.ReadPage<TeamMember, int>(pagedResult);

                Assert.AreEqual(pageNumber, pagedResult.PageNumber);
                Assert.AreEqual(expRowCount, pagedResult.RowCount);

                Assert.AreEqual(expPageCount, pagedResult.PageCount);

                var expPageSize = pageNumber < expPageCount || 0 == expPageCount % pagedResult.PageSize
                    ? pagedResult.PageSize
                    : expRowCount % pagedResult.PageSize;
                Assert.AreEqual(expPageSize, pagedResult.Results.Count());
                for (var i = 0; i < expPageSize; i++) {
                    var index = (pagedResult.PageNumber - 1) * pagedResult.PageSize + i;
                    Console.WriteLine($"Id {pagedResult.Results[i].Id} {pagedResult.Results[i].UserId}");
                    AssertTeamMember(expTeamMembers[index], pagedResult.Results[i]);
                }
            }
        }

        [Test]
        [Order(8)]
        public void TestPagedResultSorted() {
            var random = new Random(DateTime.Now.GetHashCode());
            var teamMembers = new List<TeamMember>();
            while (teamMembers.Count < 25) {
                var index = random.Next(9999);
                var administrator = CreateTeamMember<TeamMember>(index);
                if (teamMembers.All(a => a.UserId != administrator.UserId)) {
                    teamMembers.Add(administrator);
                }
            }

            var createResults = DbAccess.Create<TeamMember, int>(teamMembers);
            Assert.True(createResults.Success);
            var expTeamMembers = teamMembers.OrderBy(a => a.UserId).ToList();
            var sortFilter = new SortFilterString<TeamMember> {
                Expression = p => p.UserId
            };
            var expRowCount = teamMembers.Count;
            var expPageCount = expRowCount / PagedResult<TeamMember>.PageSize_10;
            if (0 < expRowCount % PagedResult<TeamMember>.PageSize_10) {
                expPageCount++;
            }

            for (var pageNumber = 1; pageNumber <= expPageCount; pageNumber++) {
                Console.WriteLine($"Page {pageNumber}");
                var pagedResult = new PagedResult<TeamMember> {
                    SortFilters = new List<SortFilter<TeamMember>> {sortFilter},
                    PageNumber = pageNumber
                };
                DbAccess.ReadPage<TeamMember, int>(pagedResult);

                Assert.AreEqual(pageNumber, pagedResult.PageNumber);
                Assert.AreEqual(expRowCount, pagedResult.RowCount);

                Assert.AreEqual(expPageCount, pagedResult.PageCount);

                var expPageSize = pageNumber < expPageCount || 0 == expPageCount % pagedResult.PageSize
                    ? pagedResult.PageSize
                    : expRowCount % pagedResult.PageSize;
                Assert.AreEqual(expPageSize, pagedResult.Results.Count());
                for (var i = 0; i < expPageSize; i++) {
                    var index = (pagedResult.PageNumber - 1) * pagedResult.PageSize + i;
                    Console.WriteLine(pagedResult.Results[i].UserId);
                    AssertTeamMember(expTeamMembers[index], pagedResult.Results[i]);
                }
            }

            sortFilter.Descending = true;
            expTeamMembers = teamMembers.OrderByDescending(a => a.UserId).ToList();
            for (var pageNumber = 1; pageNumber <= expPageCount; pageNumber++) {
                Console.WriteLine($"Page {pageNumber}");
                var pagedResult = new PagedResult<TeamMember> {
                    SortFilters = new List<SortFilter<TeamMember>> {sortFilter},
                    PageNumber = pageNumber
                };
                DbAccess.ReadPage<TeamMember, int>(pagedResult);
                var expPageSize = pageNumber < expPageCount || 0 == expPageCount % pagedResult.PageSize
                    ? pagedResult.PageSize
                    : expRowCount % pagedResult.PageSize;
                for (var i = 0; i < expPageSize; i++) {
                    var index = (pagedResult.PageNumber - 1) * pagedResult.PageSize + i;
                    Console.WriteLine(pagedResult.Results[i].UserId);
                    AssertTeamMember(expTeamMembers[index], pagedResult.Results[i]);
                }
            }
        }

        [Test]
        [Order(9)]
        public void TestPagedResultFiltered() {
            var teamMembers = CreateEntities(CreateTeamMember<TeamMember>, 100);

            var createResults = DbAccess.Create<TeamMember, int>(teamMembers);
            Assert.True(createResults.Success);
            var expTeamMembers = teamMembers
                .Where(a => a.UserId.Contains("9"))
                .OrderByDescending(a => a.Id).ToList();

            var expRowCount = expTeamMembers.Count;
            var expPageCount = expRowCount / PagedResult<TeamMember>.PageSize_05;
            if (0 < expRowCount % PagedResult<TeamMember>.PageSize_05) {
                expPageCount++;
            }

            for (var pageNumber = 1; pageNumber <= expPageCount; pageNumber++) {
                Console.WriteLine($"Page {pageNumber}");
                var pagedResult = new PagedResult<TeamMember> {
                    PageSize = PagedResult<TeamMember>.PageSize_05,
                    Filter = a => a.UserId.Contains("9"),
                    PageNumber = pageNumber
                };
                DbAccess.ReadPage<TeamMember, int>(pagedResult);

                Assert.AreEqual(pageNumber, pagedResult.PageNumber);
                Assert.AreEqual(expRowCount, pagedResult.RowCount);

                Assert.AreEqual(expPageCount, pagedResult.PageCount);

                var expPageSize = pageNumber < expPageCount || 0 == expPageCount % pagedResult.PageSize
                    ? pagedResult.PageSize
                    : expRowCount % pagedResult.PageSize;
                Assert.AreEqual(expPageSize, pagedResult.Results.Count());
                for (var i = 0; i < expPageSize; i++) {
                    var index = (pagedResult.PageNumber - 1) * pagedResult.PageSize + i;
                    Console.WriteLine(pagedResult.Results[i].UserId);
                    AssertTeamMember(expTeamMembers[index], pagedResult.Results[i]);
                }
            }
        }

        [Test]
        [Order(11)]
        public void TestCreateTeamMemberUserId() {
            var teamMember = CreateTeamMember<TeamMember>();
            var targetFunction =
                new Func<TeamMember, DbResult<TeamMember>>(DbAccess.Create<TeamMember, int>);
            ValidateRequiredStringTests<TeamMember, int>(teamMember, "UserId", DmConstants.MaxLength_8,
                targetFunction);
        }

        [Test]
        [Order(12)]
        public void TestCreateTeamMemberName() {
            var createFunction =
                new Func<int, TeamMember>(CreateTeamMember<TeamMember>);
            var targetFunction =
                new Func<TeamMember, DbResult<TeamMember>>(DbAccess.Create<TeamMember, int>);
            ValidateNullableStringTests<TeamMember, int>("Name", DmConstants.MaxLength_256,
                createFunction, targetFunction);
        }

        [Test]
        [Order(13)]
        public void TestCreateTeamMemberEmail() {
            var teamMember = CreateTeamMember<TeamMember>();
            var targetFunction =
                new Func<TeamMember, DbResult<TeamMember>>(DbAccess
                    .Create<TeamMember, int>);
            ValidateRequiredEmailTests<TeamMember, int>(teamMember, "Email", DmConstants.MaxLength_1024,
                targetFunction);
        }

        [Test]
        [Order(21)]
        public void TestUpdateTeamMemberUserId() {
            var teamMember = CreateTeamMember<TeamMember>();
            var addResult = DbAccess.Create<TeamMember, int>(teamMember);
            Assert.True(addResult.Success);

            var targetFunction =
                new Func<TeamMember, DbResult<TeamMember>>(DbAccess
                    .Update<TeamMember, int>);
            ValidateRequiredStringTests<TeamMember, int>(teamMember, "UserId", DmConstants.MaxLength_8, targetFunction);
        }

        [Test]
        [Order(22)]
        public void TestUpdateTeamMemberName() {
            var teamMember = CreateTeamMember<TeamMember>();
            var addResult = DbAccess.Create<TeamMember, int>(teamMember);
            Assert.True(addResult.Success);

            var targetFunction =
                new Func<TeamMember, DbResult<TeamMember>>(DbAccess
                    .Update<TeamMember, int>);
            ValidateNullableStringTests<TeamMember, int>(teamMember, "Name", DmConstants.MaxLength_256, targetFunction);
        }

        [Test]
        [Order(23)]
        public void TestUpdateTeamMemberEmail() {
            var administrator = CreateTeamMember<TeamMember>();
            var addResult = DbAccess.Create<TeamMember, int>(administrator);
            Assert.True(addResult.Success);

            var targetFunction =
                new Func<TeamMember, DbResult<TeamMember>>(DbAccess
                    .Update<TeamMember, int>);
            ValidateRequiredEmailTests<TeamMember, int>(administrator, "Email", DmConstants.MaxLength_1024,
                targetFunction);
        }
    }
}