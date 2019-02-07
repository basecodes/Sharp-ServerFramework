using System;
using System.Collections.Concurrent;
using Ssc.SscLog;
using Ssm.SsmCache;
using Ssm.SsmManager;
using Sss.SssScripts.Lua;

namespace Ssf.SsfManager {
    public class CacheManager : ICacheManager {
        private static readonly Logger Logger = LogManager.GetLogger<CacheManager>(LogType.Middle);
        private readonly ConcurrentDictionary<string, ICache> _caches;

        public CacheManager() {
            _caches = new ConcurrentDictionary<string, ICache>();

            LuaHelper.RegisterType<CacheManager>();
        }

        public ICache GetCache(string typeName) {
            if (!_caches.TryGetValue(typeName, out var value)) Logger.Warn($"获取{typeName}缓存失败！");
            return value;
        }

        public T GetCache<T>(string typeName) where T : class, ICache {
            return GetCache(typeName) as T;
        }

        public T GetCache<T>() where T : class, ICache {
            return GetCache(typeof(T).Name) as T;
        }

        public void AddCache<T>(T value) where T : class, ICache {
            AddCache(typeof(T), value);
        }

        public void AddCache(Type type, ICache value) {
            AddCache(type.Name, value);
        }

        public void AddCache(string typeName, ICache value) {
            if (!_caches.TryAdd(typeName, value)) {
                Logger.Warn($"添加{typeName}缓存失败！");
                return;
            }

            LuaHelper.RegisterType(value.GetType());
        }

        public void RemoveCache(string typeName) {
            if (!_caches.TryRemove(typeName, out _)) Logger.Warn($"移除{typeName}缓存失败！");
        }
    }
}