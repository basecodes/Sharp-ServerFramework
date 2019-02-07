using System.Collections.Generic;
using Ssc.SscConfiguration;

namespace Ssf.SsfConfiguration {

    public class NetworkConfig {

        public StreamConfig StreamConfig { get; set; } = Ssc.Ssci.StreamConfig;
        public List<SocketConfig> Services { get; set; } = new List<SocketConfig>();

        public override string ToString() {
            var sc = StreamConfig.ToString();
            var fmt = sc+"";
            foreach (var item in Services) {
                fmt += item.ToString();
            }
            return fmt;
        }
    }
}