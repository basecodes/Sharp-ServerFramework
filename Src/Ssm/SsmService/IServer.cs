using System;
using System.Collections.Generic;
using Ssc.SscConfiguration;
using Ssc.SscSerialization;
using Ssm.Ssm;
using Ssm.SsmModule;

namespace Ssm.SsmService {
    
    public interface IServer {
        
        string ID { get; }
        SocketConfig SocketConfig { get; }
        List<string> RpcMethodIds { get; }
        List<string> RpcPacketTypes { get; }
        // 事件监听
        void AddEventListener(string eventType, Action<IUser, object> listener);
        void RemoveEventListener(string eventType);
        void PostNotification(string eventType, IUser player, object arg);

        void SetHandler(ushort opCode, Action<IUser, IDeserializable> action);
        
        void AddDependency<T>() where T :  class,IModule;
        void AddDependency<T>(params object[] args) where T : class,IModule;
        
        void ClearModule(IModule module);
    }
}