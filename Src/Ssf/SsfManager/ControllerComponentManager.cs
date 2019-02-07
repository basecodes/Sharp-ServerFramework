using System;
using System.Collections.Concurrent;
using Ssc.SscLog;
using Ssm.SsmComponent;
using Ssm.SsmManager;
using Sss.SssScripts.Lua;

namespace Ssf.SsfManager {
    public class ControllerComponentManager : IControllerComponentManager {
        
        private static readonly Logger Logger = LogManager.GetLogger<ControllerComponentManager>(LogType.Middle);
        private readonly ConcurrentDictionary<string, IControllerComponent> _components;

        public ControllerComponentManager() {
            _components = new ConcurrentDictionary<string, IControllerComponent>();
            LuaHelper.RegisterType<ControllerComponentManager>();
        }

        public Implement AddControllerComponent<Interface, Implement>(Implement implement)
            where Interface : IControllerComponent
            where Implement : class, Interface {
            if (implement == null) {
                throw new ArgumentNullException(nameof(implement));
            }
            return AddControllerComponent(typeof(Interface).Name,implement);
        }

        public Implement AddControllerComponent<Implement>(string interfaceName,Implement implement)
            where Implement : class, IControllerComponent {
            if(string.IsNullOrEmpty(interfaceName)) {
                throw new ArgumentNullException(nameof(interfaceName));
            }
            
            if (implement == null) {
                throw new ArgumentNullException(nameof(implement));
            }
            
            var component = _components.GetOrAdd(interfaceName, implement);
            return component as Implement;
        }
        
        public IControllerComponent AddControllerComponent(string interfaceName,IControllerComponent controllerComponent) {
            if (controllerComponent == null) {
                throw new ArgumentNullException(nameof(controllerComponent));
            }
            var component = _components.GetOrAdd(interfaceName, controllerComponent);
            return component;
        }



        public Interface GetControllerComponent<Interface>()
            where Interface : class, IControllerComponent {
            _components.TryGetValue(typeof(Interface).Name, out var rpcComponent);
            return rpcComponent as Interface;
        }

        public Implement GetControllerComponent<Implement>(string interfaceName)
            where Implement : class, IControllerComponent {
            _components.TryGetValue(interfaceName, out var rpcComponent);
            return rpcComponent as Implement;
        }

        public IControllerComponent GetControllerComponent(string interfaceName) {
            _components.TryGetValue(interfaceName, out var rpcComponent);
            return rpcComponent;
        }

        public void RemoveControllerComponent<Interface>()
            where Interface : class, IControllerComponent {
            if (!_components.TryRemove(typeof(Interface).Name, out _)) {
                Logger.Warn($"删除{typeof(Interface).Name}失败！");
            }
        }

        public void RemoveControllerComponent(string interfaceName) {
            if (!_components.TryRemove(interfaceName, out _)) {
                Logger.Warn($"删除{interfaceName}失败！");
            }
        }
    }
}