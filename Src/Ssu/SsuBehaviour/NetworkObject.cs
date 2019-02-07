using Ssc.Ssc;
using Ssc.SscConfiguration;
using Ssc.SscLog;
using Ssc.SscStream;
using Ssu.SsuNetwork;
using Ssu;
using Ssu.Ssu;

namespace Ssu.SsuBehaviour {

    public class NetworkObject : SsuMonoBehaviour {
        private static readonly Logger Logger = LogManager.GetLogger<NetworkObject>(LogType.Middle);

        private static bool _created;
        private ClientSocket _clientSocket;
        private UpdateRunner _updateRunner;

        public bool IsEncrypt = true;
        public LogConfig LogConfig = LogManager.LogConfig;
        public LogLevel LogLevel = LogLevel.Info;
        public NetworkConfig NetworkConfig = new NetworkConfig();

        public Callback Callback;
        private void Awake() {
            if (_created) {
                return;
            }
            DontDestroyOnLoad(gameObject);
            _created = true;
            Ssui.Initialize();

        }
        
        void Start() {

            Logger.LogLevel = LogLevel;
            Ssui.Security.IsEncrypt = IsEncrypt;

            _updateRunner = new UpdateRunner();
            _clientSocket = new ClientSocket(NetworkConfig,_updateRunner);
            _clientSocket.Reconnect((state)=> Callback?.ReconnectEvent?.Invoke(state));

            StartConnection();
        }

        private void StartConnection() {
            _clientSocket.Connected += Connected;
            _clientSocket.Disconnected += Disconnected;

            Connect();
        }

        public void Connect() {
           Connect(NetworkConfig.ServerIp, NetworkConfig.ServerPort);
        }

        public void Connect(string ip,int port) {
            _clientSocket.Connect(ip, port);
        }

        private void Disconnected(IPeer peer) {
            Logger.Info($"连接断开!");
            Callback?.DisconnectEvent?.Invoke();
            //peer.Disconnect();
        }

        private void Connected(IPeer peer, IReadStream readStream) {
            if (peer == null) {
                Logger.Info($"连接失败！");
                Callback?.ConnectEvent?.Invoke(false,readStream);
                return;
            }

            Logger.Info("连接到: " + peer.Connection.RemoteAddress);
            var ids = readStream.ShiftRight<string>();

            Logger.Info(ids.Length);
            if (ids != null && ids.Length != 0) {
                var rpcMethodIds = ids.Split(';');

                foreach (var item in rpcMethodIds) {
                    Ssui.Connections[item] = peer;
                }

                Logger.Info(ids);
            }

            Callback?.ConnectEvent?.Invoke(true,readStream);
        }

        private void Update() {
            _updateRunner.Update();
        }

        private void OnApplicationQuit() {
            _clientSocket.Dispose();
        }

        protected override void OnDestory() {
            base.OnDestory();
            _clientSocket.Connected -= Connected;
            _clientSocket.Disconnected -= Disconnected;
        }
    }
}