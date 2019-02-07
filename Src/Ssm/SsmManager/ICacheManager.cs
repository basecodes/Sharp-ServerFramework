using System;
using Ssm.SsmCache;

namespace Ssm.SsmManager {
    public interface ICacheManager {
        void AddCache(string typeName, ICache value);
        void AddCache(Type type, ICache value);
        void AddCache<T>(T value) where T : class, ICache;
        ICache GetCache(string typeName);
        T GetCache<T>() where T : class, ICache;
        T GetCache<T>(string typeName) where T : class, ICache;
        void RemoveCache(string typeName);
    }
}