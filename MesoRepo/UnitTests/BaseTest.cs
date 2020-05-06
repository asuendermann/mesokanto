﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons.Configuration;
using Commons.DomainModel.Base;
using Commons.DomainModel.Domain;
using DatabaseAccess;
using DomainModel.Administration;
using DomainModel.Base;
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
            foreach (var administrator in DbAccess.Set<Administrator>().ToList()) {
                DbAccess.Remove(administrator);
            }
            foreach (var project in DbAccess.Set<Project>().ToList()) {
                DbAccess.Remove(project);
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

        public static void AssertAdministrator<T1, T2>(T1 expAdministrator, T2 administrator)
            where T1 : IAdministrator
            where T2 : IAdministrator {
            if (null == administrator || null == expAdministrator) {
                Assert.Fail();
            }

            Assert.AreEqual(expAdministrator.UserIdentityName, administrator.UserIdentityName);
            Assert.AreEqual(expAdministrator.Name, administrator.Name);
            Assert.AreEqual(expAdministrator.Email, administrator.Email);
            Assert.AreEqual(expAdministrator.Phone, administrator.Phone);
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

        public static T CreateAdministrator<T>(int index = 1)
            where T : IAdministrator, new() {
            var administrator = new T {
                UserIdentityName = $@"NU\{typeof(T).Name}_{index:D4}",
                Name = $"ncoretest_name {index}",
                Email = $"ncoretest.{typeof(T).Name}_{index}@ncoretest.com",
                Phone = $"{index:D4}"
            };
            return administrator;
        }

        public static T CreateProject<T>(int index = 1)
            where T : IProject, new() {
            var project = new T {
                Name = $"Project {index:d4}",
                Description = $"Description of Project {index:D4}"
            };
            return project;
        }

        public static void ModifyAdministrator<T>(T administrator, T expAdministrator)
            where T : IAdministrator {
            if (null != administrator && null != expAdministrator) {
                administrator.UserIdentityName = expAdministrator.UserIdentityName;
                administrator.Name = expAdministrator.Name;
                administrator.Email = expAdministrator.Email;
                administrator.Phone = expAdministrator.Phone;
            }
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