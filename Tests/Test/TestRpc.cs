using NUnit.Framework;
using Ssc;
using Ssc.Ssc;
using Ssc.SscAttribute;
using Ssc.SscStream;
using Ssc.SscTemplate;

namespace Test {

    public interface ITest {
        [RpcRequest("111")]
        void Test(int num);
    }

    [TestFixture]
    public class TestRpc:Controller {

        [Test]
        public void TestRpcService() {
            PoolAllocator<IWriteStream>.SetPool((arguments => new WriteStream()));
            Ssci.Invoke<ITest>(() => (rpc) => rpc.Test(200), null, null);
        }
    }
}
