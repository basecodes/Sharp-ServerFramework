using System.Collections.Generic;
using Ssc.SscLog;
using Ssm.SsmService;

namespace Ssm.SsmManager {
    public class ServiceManager {

        private static readonly Dictionary<string, IServer> _services;
        private static readonly Logger Logger = LogManager.GetLogger<ServiceManager>(LogType.Middle);
        static ServiceManager() {
            _services = new Dictionary<string, IServer>();
        }

        public static IServer GetService(string name) {
            _services.TryGetValue(name, out var result);
            return result;
        }

        public static void AddService(string name,IServer server) {
            if (_services.ContainsKey(name)) {
                Logger.Warn($"Name:{name}服务器已经添加！");
                return;
            }
            _services.Add(name,server);
        }

        public static void RemoveService(string name) {
            if (!_services.ContainsKey(name)) {
                Logger.Warn($"Name:{name}服务器已经删除！");
                return;
            }
            _services.Remove(name);
        }
    }
}
