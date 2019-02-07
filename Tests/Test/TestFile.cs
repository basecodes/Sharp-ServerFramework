using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Ssc.SscConfiguration;
using Ssc.SscFile;
using Ssf;

namespace Test {
    [TestFixture]
    public class TestFile {
        [Test]
        public void TestJson() {
            var networkConfig = Ssfi.NetworkConfig;
            networkConfig.Services.Add(new SocketConfig());
            networkConfig.Services.Add(new SocketConfig());
            var json = JsonHelper.ToJson(networkConfig);
            Assert.NotNull(json);
        }

        [Test]
        public void TestEncryptor() {
            var json = JsonHelper.ToJson(Ssfi.CryptoConfig);
            Assert.NotNull(json);
        }
    }
}
