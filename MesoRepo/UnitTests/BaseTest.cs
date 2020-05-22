using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons.Configuration;
using Commons.DomainModel.Base;
using Commons.DomainModel.Scrum;
using DatabaseAccess;
using DomainModel.Base;
using DomainModel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace UnitTests {
    public class BaseTest {
        protected readonly DbContext DbAccess;

        protected readonly string Requestor = "MesoRepo.UnitTests";

        protected readonly IServiceProvider ServiceProvider = DependencyInjector.GetServiceProvider();

        protected BaseTest() {
            var configurationService = ServiceProvider.GetService<IConfigurationService>();
            if (null == configurationService) {
                throw new ArgumentNullException(nameof(IConfigurationService));
            }

            DbAccess = ServiceProvider.GetService<BaseDbContext>();
            if (null == DbAccess) {
                throw new ArgumentNullException(nameof(BaseDbContext));
            }

            DbContextExtensions.Requestor = Requestor;

            //DbAccess.Database.EnsureDeleted();
            DbAccess.Database.EnsureCreated();
        }

        [SetUp]
        public virtual void Setup() {
            DbAccess.DetachAllEntities();
            foreach (var project in DbAccess.Set<Project>().ToList()) {
                DbAccess.Remove(project);
            }

            foreach (var teamMember in DbAccess.Set<TeamMember>().ToList()) {
                DbAccess.Remove(teamMember);
            }

            DbAccess.SaveChanges();
        }

        protected void AssertBaseCreated<TId>(IAuditable<TId> entity) {
            if (null == entity) {
                Assert.Fail();
            }

            Assert.AreNotEqual(default(int), entity.Id);
            Assert.AreNotEqual(default(DateTime), entity.CreatedAt);
            Assert.AreEqual(Requestor, entity.CreatedBy);
            Assert.IsNull(entity.ModifiedAt);
            Assert.IsNull(entity.ModifiedBy);
        }

        protected void AssertBaseModified<TId>(IAuditable<TId> auditableBase) {
            Assert.IsNotNull(auditableBase.ModifiedAt);
            Assert.AreNotEqual(default(DateTime), auditableBase.ModifiedAt);
            Assert.AreEqual(auditableBase.ModifiedBy, Requestor);
        }

        public void AssertTablePerHierarchy<T, TId>(ITablePerHierarchy<TId> entity) {
            if (null != entity) {
                Assert.AreEqual(typeof(T).Name, entity.Discriminator);
            }
        }

        public static void AssertProject(Project expProject, Project project) {
            if (null == project || null == expProject) {
                Assert.Fail();
            }

            Assert.AreEqual(expProject.Identifier, project.Identifier);
            Assert.AreEqual(expProject.Title, project.Title);
            Assert.AreEqual(expProject.Description, project.Description);

            var expPtms = expProject.ProjectTeamMembers?.ToList() ?? new List<ProjectTeamMember>();
            var ptms = project.ProjectTeamMembers?.ToList() ?? new List<ProjectTeamMember>();
            if (expPtms.Any()) {
                Assert.AreEqual(expPtms.Count(), ptms.Count());
                foreach (var expPtm in expPtms) {
                    var ptm = ptms.FirstOrDefault(p => p.TeamMemberId == expPtm.TeamMemberId);
                    Assert.NotNull(ptm);
                    AssertTeamMember(expPtm.TeamMember, ptm.TeamMember);
                }
            } else {
                Assert.False(ptms.Any());
            }

        }

        public static void AssertTeamMember<T1, T2>(T1 expTeamMember, T2 teamMember)
            where T1 : ITeamMember
            where T2 : ITeamMember {
            if (null == teamMember || null == expTeamMember) {
                Assert.Fail();
            }

            Assert.AreEqual(expTeamMember.Name, teamMember.Name);
            Assert.AreEqual(expTeamMember.Email, teamMember.Email);
        }

        protected void ChangeTypeOfEntry<T1, T2, TId>(T1 entry)
            where T1 : ITablePerHierarchy<TId>
            where T2 : class, ITablePerHierarchy<TId> {
            if (null != entry) {
                entry.Discriminator = typeof(T2).Name;
                Assert.AreEqual(1, DbAccess.SaveChanges());
                DbAccess.DetachAllEntities();
                Assert.AreEqual(0, DbAccess.SaveChanges());
            }
        }

        protected ICollection<T> CreateEntities<T>(Func<int, T> initializerFunction, int count)
            where T : class, new() {
            var entities = new List<T>();
            for (var index = 1; index <= count; index++) {
                var entity = initializerFunction(index);
                entities.Add(entity);
            }

            return entities;
        }

        public static T CreateProject<T>(int index = 1)
            where T : IProject, new() {
            var project = new T {
                Identifier = $"{DateTime.Today.Year}-{index:d4}",
                Title = $"Project {index:d4}",
                Description = $"Description of Project {index:D4}"
            };
            return project;
        }

        public static T CreateTeamMember<T>(int index = 1)
            where T : ITeamMember, new() {
            var teamMember = new T {
                UserId = $"UID_{index:d4}",
                Name = $"Project {index:d4}",
                Email = $"email.{index:D4}@mesorepo.com"
            };
            return teamMember;
        }

        public static void ModifyProject<T>(T project, T expProject)
            where T : IProject {
            if (null != project && null != expProject) {
                project.Identifier = expProject.Identifier;
                project.Title = expProject.Title;
                project.Description = expProject.Description;
            }
        }

        public static void ModifyTeamMember<T>(T teamMember, T expTeamMember)
            where T : ITeamMember {
            if (null != teamMember && null != expTeamMember) {
                teamMember.UserId = expTeamMember.UserId;
                teamMember.Name = expTeamMember.Name;
                teamMember.Email = expTeamMember.Email;
            }
        }

        protected void ValidateNullableStringTests<T, TId>(
            string propertyName,
            int maxLength,
            Func<int,T> createFunction,
            Func<T, DbResult<T>> targetFunction)
            where T : BaseAuditable<TId> {
            DbResult<T> TargetFunction(T entity ) {
                return targetFunction(entity);
            }

            var propertyInfo = typeof(T).GetProperty(propertyName);
            Assert.NotNull(propertyInfo);

            var entity1 = createFunction(1);
            propertyInfo.SetValue(entity1, null);
            var resultNull = TargetFunction(entity1);
            Assert.True(resultNull.Success);

            var entity2 = createFunction(2);
            propertyInfo.SetValue(entity2, string.Empty);
            var resultEmpty = TargetFunction(entity2);
            Assert.True(resultEmpty.Success);

            var entity3 = createFunction(3);
            propertyInfo.SetValue(entity3, CreateString(maxLength));
            var resultMaxLength = TargetFunction(entity3);
            Assert.True(resultMaxLength.Success);

            var entity4 = createFunction(4);
            propertyInfo.SetValue(entity4, CreateString(maxLength + 1));
            var resultTooLong = TargetFunction(entity4);
            Assert.AreEqual(DbResultCode.ValidationFailed, resultTooLong.ResultCode);
        }

        protected void ValidateNullableStringTests<T, TId>(
            T entity,
            string propertyName,
            int maxLength,
            Func<T, DbResult<T>> targetFunction)
            where T : BaseAuditable<TId> {
            DbResult<T> TargetFunction() {
                return targetFunction(entity);
            }

            var propertyInfo = typeof(T).GetProperty(propertyName);
            Assert.NotNull(propertyInfo);

            propertyInfo.SetValue(entity, null);
            var resultNull = TargetFunction();
            Assert.True(resultNull.Success);

            propertyInfo.SetValue(entity, string.Empty);
            var resultEmpty = TargetFunction();
            Assert.True(resultEmpty.Success);

            propertyInfo.SetValue(entity, CreateString(maxLength));
            var resultMaxLength = TargetFunction();
            Assert.True(resultMaxLength.Success);

            propertyInfo.SetValue(entity, CreateString(maxLength + 1));
            var resultTooLong = TargetFunction();
            Assert.AreEqual(DbResultCode.ValidationFailed, resultTooLong.ResultCode);
        }


        protected void ValidateRequiredStringTests<T, TId>(
            T entity,
            string propertyName,
            int maxLength,
            Func<T, DbResult<T>> targetFunction)
            where T : BaseAuditable<TId> {
            DbResult<T> TargetFunction() {
                return targetFunction(entity);
            }

            var propertyInfo = typeof(T).GetProperty(propertyName);
            Assert.NotNull(propertyInfo);

            propertyInfo.SetValue(entity, null);
            var resultNull = TargetFunction();
            Assert.AreEqual(DbResultCode.ValidationFailed, resultNull.ResultCode);

            propertyInfo.SetValue(entity, string.Empty);
            var resultEmpty = TargetFunction();
            Assert.AreEqual(DbResultCode.ValidationFailed, resultEmpty.ResultCode);

            propertyInfo.SetValue(entity, CreateString(maxLength + 1));
            var resultTooLong = TargetFunction();
            Assert.AreEqual(DbResultCode.ValidationFailed, resultTooLong.ResultCode);

            propertyInfo.SetValue(entity, CreateString(maxLength));
            var resultMaxLength = TargetFunction();
            Assert.True(resultMaxLength.Success);
        }

        protected static void ValidateRequiredEmailTests<T, TId>(
            T entity,
            string propertyName,
            int maxLength,
            Func<T, DbResult<T>> targetFunction)
            where T : BaseAuditable<TId> {
            DbResult<T> TargetFunction() {
                return targetFunction(entity);
            }

            var propertyInfo = typeof(T).GetProperty(propertyName);
            Assert.NotNull(propertyInfo);

            propertyInfo.SetValue(entity, null);
            var resultNull = TargetFunction();
            Assert.AreEqual(DbResultCode.ValidationFailed, resultNull.ResultCode);

            propertyInfo.SetValue(entity, string.Empty);
            var resultEmpty = TargetFunction();
            Assert.AreEqual(DbResultCode.ValidationFailed, resultEmpty.ResultCode);

            propertyInfo.SetValue(entity, CreateString(32));
            var resultNotEmail = TargetFunction();
            Assert.AreEqual(DbResultCode.ValidationFailed, resultNotEmail.ResultCode);

            propertyInfo.SetValue(entity, CreateEmailString(maxLength + 1));
            var resultTooLong = TargetFunction();
            Assert.AreEqual(DbResultCode.ValidationFailed, resultTooLong.ResultCode);

            propertyInfo.SetValue(entity, "abc@test..de");
            var resultIllFormat = TargetFunction();
            Assert.AreEqual(DbResultCode.ValidationFailed, resultIllFormat.ResultCode);

            propertyInfo.SetValue(entity, CreateEmailString(maxLength));
            var resultMaxLength = TargetFunction();
            Assert.True(resultMaxLength.Success);
        }

        protected static string CreateString(int length) {
            var sb = new StringBuilder();
            for (var i = 1; i <= length; i++) {
                sb.Append(i % 10);
            }

            return sb.ToString();
        }

        protected static string CreateEmailString(int maxLength) {
            var sb = new StringBuilder();
            for (var i = 1; i <= maxLength - 7; i++) {
                sb.Append(i % 10);
            }

            sb.Append("@abc.xy");
            return sb.ToString();
        }
    }
}