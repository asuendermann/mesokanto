using System;
using System.Linq;
using System.Linq.Expressions;

using HelloCoreCommons.DomainModel;

using HelloCoreDal.DomainModel;
using HelloCoreDal.Repository;

using NUnit.Framework;

namespace HelloCoreTest.DAL {
    public class TestRepoAdministrators : BaseDataLayerTest {

        [Test]
        public void TestReadMasterAdministrators() {
            var expMas = PopulateAdministrators<MasterAdministrator>(3);
            var expPas = PopulateAdministrators<ProjectAdministrator>(3);
            AssertRead<Administrator,int>(6);
            AssertRead<MasterAdministrator, int>(3);
            AssertRead<ProjectAdministrator, int>(3);
            AssertReadWithPredicate<Administrator, int>(a => a.UserIdentityName == expPas[0].UserIdentityName);
            AssertReadWithPredicate<MasterAdministrator, int>(a => a.UserIdentityName == expMas[0].UserIdentityName);
            AssertReadWithPredicate<ProjectAdministrator, int>(a => a.UserIdentityName == expPas[0].UserIdentityName);
        }

        private void AssertRead<T,TId>(int expCount)
            where T : class, IAdministrator<TId> {
            var repository =
                ServiceProvider.GetService(typeof(IGenericRepository<T, TId>)) as IGenericRepository<T, TId>;
            Assert.NotNull(repository);
            var resultsList = repository.Read();
            Assert.AreEqual(expCount, resultsList.Count());
        }

        private void AssertReadWithPredicate<T, TId>(Expression<Func<T, bool>> predicate, int expCount = 1 )
            where T : class,IAdministrator<TId> {
            var repository =
                ServiceProvider.GetService(typeof(IGenericRepository<T, TId>)) as IGenericRepository<T, TId>;
            Assert.NotNull(repository);
            var resultsList = repository.Read(predicate);
            Assert.AreEqual(expCount, resultsList.Count());
        }
    }
}