using DatabaseAccess.Repository;
using DomainModel.Administration;

using NUnit.Framework;

using Repository;

namespace UnitTests {
    public class BaseRepositoryTest {
        [Test]
        public void TestResultCodes() {
            var result = new RepositoryResult<Administrator>();
            result.ResultCode |= RepositoryResultCode.Impractical;
            Assert.IsTrue(result.Impractical);
            result.ResultCode |= RepositoryResultCode.Invalid;
            Assert.IsTrue(result.Impractical);
            Assert.IsTrue(result.Invalid);
            result.ResultCode |= RepositoryResultCode.Duplicate;
            Assert.IsTrue(result.Impractical);
            Assert.IsTrue(result.Invalid);
            Assert.IsTrue(result.Duplicate);
            result.ResultCode |= RepositoryResultCode.SaveFailed;
            Assert.IsTrue(result.Impractical);
            Assert.IsTrue(result.Invalid);
            Assert.IsTrue(result.Duplicate);
            Assert.IsTrue(result.SaveFailed);
        }
    }
}