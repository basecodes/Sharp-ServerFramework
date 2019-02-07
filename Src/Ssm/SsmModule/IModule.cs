using System;
using System.Collections.Generic;
using Ssc.SscStream;
using Ssm.Ssm;
using Ssm.SsmManager;
using Ssm.SsmService;

namespace Ssm.SsmModule {
    public interface IModule : IDisposable {
        string ServiceId { get; }
        List<string> RpcMethodIds { get; }
        List<string> RpcPacketTypes { get; }
        void Dispose(ICacheManager cacheManager, IControllerComponentManager controllerComponentManager);
        void Initialize(IServer server, ICacheManager cacheManager, IControllerComponentManager controllerComponentManager);
        void InitFinish(IServer server, ICacheManager cacheManager, IControllerComponentManager controllerComponentManager);
        void Finish(IServer server, ICacheManager cacheManager, IControllerComponentManager controllerComponentManager);

        bool Accepted(IUser peer, IReadStream readStream, IWriteStream writeStream);
        void Connected(IUser peer,IReadStream readStream);
        void Disconnected(IUser peer);
    }
}