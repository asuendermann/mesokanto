using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using HelloCoreCommons.Attributes;
using HelloCoreCommons.Configuration;
using HelloCoreCommons.DomainModel;

using HelloCoreDal.DataAccessLayer;
using HelloCoreDal.DomainModel;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using Serilog;

namespace HelloCoreTest.DAL {
    public abstract class BaseDataLayerTest {
        protected readonly DemoDbContext DbContext;

        protected readonly string Requestor;

        protected readonly IServiceProvider ServiceProvider = DependencyInjector.GetServiceProvider();

        protected BaseDataLayerTest() {
            var configuration = ServiceProvider.GetService(typeof(IConfigurationService)) as IConfigurationService;
            if (null == configuration) {
                throw new ArgumentNullException(typeof(IConfigurationService).Name);
            }

            Requestor = ConfigurationTk.InitialAssemblyName;

            DbContext = ServiceProvider.GetService(typeof(DemoDbContext)) as DemoDbContext;
            if (null == DbContext) {
                throw new ArgumentNullException(typeof(DemoDbContext).Name);
            }

            //DbContext.Database.EnsureDeleted();
            DbContext.Database.EnsureCreated();
        }

        [SetUp]
        public virtual void Setup() {
            DbContext.DetachAllEntities();
            foreach (var administrator in DbContext.Administrators.ToList()) {
                DbContext.Remove(administrator);
            }

            DbContext.SaveChanges();
        }

        protected void ChangeTypeOfEntry<T1, T2, TId>(T1 entry)
            where T1 : ITypePerHierarchy, IEntityBase<TId>
            where T2 : class {
            if (null != entry) {
                entry.Discriminator = typeof(T2).Name;
                Assert.AreEqual(1, DbContext.SaveChanges());
                DbContext.DetachAllEntities();
                Assert.AreEqual(0, DbContext.SaveChanges());
            }
        }

        protected IList<T> PopulateAdministrators<T>(int count)
            where T : Administrator, new() {
            var adminList = new List<T>();
            for (var index = 1; index <= count; index++) {
                var administrator = CreateAdministrator<T, int>(index);
                adminList.Add(administrator);
                DbContext.Administrators.Add(administrator);
            }

            DbContext.SaveChanges();

            return adminList;
        }

        public static T CreateAdministrator<T, TId>(int index = 1)
            where T : IAdministrator<TId>, ITypePerHierarchy, new() {
            var administrator = new T {
                UserIdentityName = $@"NU\{typeof(T).Name}_{index}",
                Name = $"ncoretest_name {index}",
                Email = $"ncoretest.{typeof(T).Name}_{index}@ncoretest.com",
                Phone = $"{index:D4}"
            };
            return administrator;
        }

        public static void ModifyAdministrator<T, TId>(T administrator, T expAdministrator)
            where T : IAdministrator<TId>, ITypePerHierarchy {
            if (null != administrator && null != expAdministrator) {
                administrator.UserIdentityName = expAdministrator.UserIdentityName;
                administrator.Name = expAdministrator.Name;
                administrator.Email = expAdministrator.Email;
                administrator.Phone = expAdministrator.Phone;
            }
        }

        public static void AssertAdministrator<T1, T2, TId>(T1 expAdministrator, T2 administrator)
            where T1 : IAdministrator<TId>
            where T2 : IAdministrator<TId> {
            if (null != administrator && null != expAdministrator) {
                Assert.AreEqual(expAdministrator.UserIdentityName, administrator.UserIdentityName);
                Assert.AreEqual(expAdministrator.Name, administrator.Name);
                Assert.AreEqual(expAdministrator.Email, administrator.Email);
                Assert.AreEqual(expAdministrator.Phone, administrator.Phone);
            }
        }

        public static void AssertTypePerHierarchy<T>(ITypePerHierarchy administrator)
            where T : ITypePerHierarchy {
            if (null != administrator) {
                Assert.AreEqual(typeof(T).Name, administrator.Discriminator);
            }
        }

        protected bool TryAddEntity<T, TId>(T entity)
            where T : IAuditableBase<TId> {
            DbContext.Entry(entity).State = EntityState.Added;
            return 1 == DbContext.SaveChanges();
        }

        protected bool TryAddAndDeleteEntity<T, TId>(T entity)
            where T : IAuditableBase<TId> {
            DbContext.Entry(entity).State = EntityState.Added;
            var added = DbContext.SaveChanges();
            DbContext.Entry(entity).State = EntityState.Deleted;
            var deleted = DbContext.SaveChanges();
            entity.Id = default;
            return 2 == added + deleted;
        }

        protected bool TryUpdateEntity<T, TId>(T entity)
            where T : IAuditableBase<TId> {
            DbContext.Entry(entity).State = EntityState.Modified;
            return 1 == DbContext.SaveChanges();
        }

        protected static void ValidateRequiredStringTests<T, TId>(
            T entity,
            string propertyName,
            int maxLength,
            Func<T, bool> invokeException)
            where T : IAuditableBase<TId> {
            void InvokeException() {
                invokeException(entity);
            }

            var propertyInfo = typeof(T).GetProperty(propertyName);
            Assert.NotNull(propertyInfo);
            propertyInfo.SetValue(entity, null);
            Assert.Throws<ValidationException>(InvokeException);
            propertyInfo.SetValue(entity, string.Empty);
            Assert.Throws<ValidationException>(InvokeException);
            propertyInfo.SetValue(entity, CreateString(maxLength + 1));
            Assert.Throws<ValidationException>(InvokeException);
            propertyInfo.SetValue(entity, CreateString(maxLength));
            Assert.True(invokeException(entity));
        }

        protected static void ValidateNullableStringTests<T, TId>(
            T entity,
            string propertyName,
            int maxLength,
            Func<T, bool> invokeException)
            where T : IAuditableBase<TId> {
            void InvokeException() {
                invokeException(entity);
            }

            var propertyInfo = typeof(T).GetProperty(propertyName);
            Assert.NotNull(propertyInfo);

            propertyInfo.SetValue(entity, null);
            Assert.True(invokeException(entity));

            propertyInfo.SetValue(entity, string.Empty);
            Assert.True(invokeException(entity));

            propertyInfo.SetValue(entity, CreateString(maxLength));
            Assert.True(invokeException(entity));

            propertyInfo.SetValue(entity, CreateString(maxLength + 1));
            Assert.Throws<ValidationException>(InvokeException);
        }


        protected static void ValidateRequiredEmailTests<T, TId>(
            T entity,
            string propertyName,
            int maxLength,
            Func<T, bool> invokeException)
            where T : IAuditableBase<TId> {
            void InvokeException() {
                invokeException(entity);
            }

            var propertyInfo = typeof(T).GetProperty(propertyName);
            Assert.NotNull(propertyInfo);
            propertyInfo.SetValue(entity, null);
            Assert.Throws<ValidationException>(InvokeException);
            propertyInfo.SetValue(entity, string.Empty);
            Assert.Throws<ValidationException>(InvokeException);
            propertyInfo.SetValue(entity, CreateString(32));
            Assert.Throws<ValidationException>(InvokeException);
            propertyInfo.SetValue(entity, CreateEmailString(maxLength + 1));
            Assert.Throws<ValidationException>(InvokeException);
            propertyInfo.SetValue(entity, "abc@test..de");
            Assert.Throws<ValidationException>(InvokeException);
            propertyInfo.SetValue(entity, CreateEmailString(maxLength));
            Assert.True(invokeException(entity));
        }

        protected void AssertBaseCreated<TId>(IAuditableBase<TId> auditableBase) {
            Assert.AreNotEqual(default(TId), auditableBase.Id);
            Assert.AreNotEqual(default(DateTime), auditableBase.CreatedAt);
            Assert.AreEqual(Requestor, auditableBase.CreatedBy);
            Assert.IsNull(auditableBase.ModifiedAt);
            Assert.IsNull(auditableBase.ModifiedBy);
        }

        protected void AssertBaseModified<TId>(IAuditableBase<TId> auditableBase) {
            Assert.IsNotNull(auditableBase.ModifiedAt);
            Assert.AreNotEqual(default(DateTime), auditableBase.ModifiedAt);
            Assert.AreEqual(auditableBase.ModifiedBy, Requestor);
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