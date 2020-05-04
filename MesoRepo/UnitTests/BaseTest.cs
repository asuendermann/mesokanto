
using Commons.Configuration;
using Commons.DomainModel.Base;
using Commons.DomainModel.Domain;
using DatabaseAccess;
using DomainModel.Administration;
using DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using System;
using System.Data.SqlClient;
using System.Linq;

namespace UnitTests {
    public class BaseTest {
        protected readonly DbContext DbAccess;

        protected readonly string Requestor = "MesoRepo.UnitTests";

        protected readonly IServiceProvider ServiceProvider = DependencyInjector.GetServiceProvider();

        protected BaseTest() {
            var configurationService = ServiceProvider.GetService<IConfigurationService>();
            if (null == configurationService) {
                throw new ArgumentNullException(typeof(IConfigurationService).Name);
            }

            DbAccess = ServiceProvider.GetService<BaseDbContext>();
            if (null == DbAccess) {
                throw new ArgumentNullException(typeof(BaseDbContext).Name);
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

            DbAccess.SaveChanges();
        }

        protected void AssertBaseCreated<TId>(IAuditable<TId> entity) {
            if ( null == entity) {
                Assert.Fail();
            }
            Assert.AreNotEqual(default(int), entity.Id);
            Assert.AreNotEqual(default(DateTime), entity.CreatedAt);
            Assert.AreEqual(Requestor, entity.CreatedBy);
            Assert.IsNull(entity.ModifiedAt);
            Assert.IsNull(entity.ModifiedBy);
        }

        public void AssertTypePerHierarchy<T,TId>( ITablePerHierarchy<TId> entity) {
            AssertBaseCreated(entity);
            if (null != entity) {
                Assert.AreEqual(typeof(T).Name, entity.Discriminator);
            }            
        }

        public static void AssertAdministrator<T1, T2>(T1 expAdministrator, T2 administrator)
            where T1 : IAdministrator
            where T2 : IAdministrator {
            if (null == administrator || null == expAdministrator ) {
                Assert.Fail();
            }
            Assert.AreEqual(expAdministrator.UserIdentityName, administrator.UserIdentityName);
            Assert.AreEqual(expAdministrator.Name, administrator.Name);
            Assert.AreEqual(expAdministrator.Email, administrator.Email);
            Assert.AreEqual(expAdministrator.Phone, administrator.Phone);
        }

        public static T CreateAdministrator<T, TId>(int index = 1)
            where T : IAdministrator, new() {
            var administrator = new T {
                UserIdentityName = $@"NU\{typeof(T).Name}_{index:D2}",
                Name = $"ncoretest_name {index}",
                Email = $"ncoretest.{typeof(T).Name}_{index}@ncoretest.com",
                Phone = $"{index:D4}"
            };
            return administrator;
        }

    }
}