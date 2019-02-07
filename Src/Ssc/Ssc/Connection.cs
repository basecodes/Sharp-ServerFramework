using System.Net;
using Ssc.SscLog;

namespace Ssc.Ssc {
    public class Connection {
        private static readonly Logger Logger = LogManager.GetLogger<Connection>(LogType.Low);
        
        public EndPoint RemoteAddress { get; set; }
        public EndPoint LocalAddress { get; set; }

        public void Clear() {
            RemoteAddress = null;
            LocalAddress = null;
        }
    }
}