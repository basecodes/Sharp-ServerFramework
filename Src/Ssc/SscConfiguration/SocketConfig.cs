namespace Ssc.SscConfiguration {

    public enum ServiceType {
        Server,
        Client
    }
    public class SocketConfig {
        public bool Enable = true;
        public ServiceType ServiceType  = ServiceType.Server;
        public string ServiceID;
        public string IP;
        public int Port;
        public bool OpenKeepAlive = true;
        public int KeepAliveInterval = 3000;
        public int ReconnectMaxCount = 3;
        public bool OpenFragmentResend = true;
        public int FragmentInterval = 2000;

        public override string ToString() {
            var str = "";
            var fields = GetType().GetFields();
            foreach (var item in fields) {
                var value = item.GetValue(this);
                str += item.Name + " : " + value + "\n";
            }
            return str;
        }
    }
}
