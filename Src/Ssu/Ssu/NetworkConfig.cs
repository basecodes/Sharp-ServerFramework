using System;
using Ssc.SscConfiguration;

namespace Ssu.Ssu {
    [Serializable]
    public class NetworkConfig {
        public StreamConfig StreamConfig = Ssc.Ssci.StreamConfig;

        public int AckInterval = 3000;
        public bool KeepAlive = true;
        public int PacketFilterCount = 10;
        public int ResendInterval = 1000;
        public int MaxReconnectCount = 3;
        public bool FragmentResend = true;

        public string ServerIp = "127.0.0.1";
        public int ServerPort = 5000;
    }
}