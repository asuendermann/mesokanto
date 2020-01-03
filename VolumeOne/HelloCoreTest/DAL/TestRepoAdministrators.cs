using System;
using System.Linq.Expressions;

using NUnit.Framework;

namespace HelloCoreTest.DAL {
    public class TestRepoAdministrators : BaseDataLayerTest {

        [Test]
        public void TestReadMasterAdministrators() {
            var expMas = PopulateAdministrators<MasterAdministrator>(3);
            var expPas = PopulateAdministrators<ProjectAdministrator>(3);
            AssertReadAdministrators<Administrator,int>(6);
            AssertReadAdministrators<MasterAdministrator, int>(3);
            AssertReadAdministrators<ProjectAdministrator, int>(3);
            AssertReadAdministrators<Administrator, int>(a => a.UserIdentityName == expMas[0].UserIdentityName);
            AssertReadAdministrators<Administrator, int>(a => a.UserIdentityName == expPas[0].UserIdentityName);
            AssertReadAdministrators<MasterAdministrator, int>(a => a.UserIdentityName == expMas[0].UserIdentityName);
            AssertReadAdministrators<ProjectAdministrator, int>(a => a.UserIdentityName == expPas[0].UserIdentityName);
        }

        private void AssertReadAdministrators<T,TId>(int expCount)
            where T : class, IAdministrator<TId> {
            var repository =
                ServiceProvider.GetService(typeof(IGenericRepository<T, TId>)) as GenericDbRepository<T, TId>;
            Assert.NotNull(repository);
            var administrators = repository.Read();
            Assert.AreEqual(expCount, administrators.Count());
        }

        private void AssertReadAdministrators<T, TId>(Expression<Func<T, bool>> predicate)
            where T : class,IAdministrator<TId> {
            var repository =
                ServiceProvider.GetService(typeof(IGenericRepository<T, TId>)) as GenericDbRepository<T, TId>;
            Assert.NotNull(repository);
            var administrators = repository.Read(predicate);
            Assert.AreEqual(1, administrators.Count());
        }
    }
}