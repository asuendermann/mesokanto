
using Commons.Configuration;

using DatabaseAccess;

using DomainModel.Administration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using System;
using System.Linq;

namespace UnitTests {
    public class BaseTest {
        protected readonly BaseDbContext DbContext;

        protected readonly string Requestor = "MesoRepo.UnitTests";

        protected readonly IServiceProvider ServiceProvider = DependencyInjector.GetServiceProvider();

        public BaseTest() {
            var configurationService = ServiceProvider.GetService<IConfigurationService>();
            if (null == configurationService) {
                throw new ArgumentNullException(typeof(IConfigurationService).Name);
            }

            DbContext = ServiceProvider.GetService<BaseDbContext>();
            if (null == DbContext) {
                throw new ArgumentNullException(typeof(BaseDbContext).Name);
            }

            //DbContext.Database.EnsureDeleted();
            DbContext.Database.EnsureCreated();
        }

        [SetUp]
        public virtual void Setup() {
            DbContext.DetachAllEntities();
            foreach (var administrator in DbContext.Set<Administrator>().ToList()) {
                DbContext.Remove(administrator);
            }

            DbContext.SaveChanges();
        }

        [Test]
        public void Test1() {
            Assert.Fail();
        }

    }
}