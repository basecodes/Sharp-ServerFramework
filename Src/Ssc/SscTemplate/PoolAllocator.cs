using Ssc.Ssc;
using Ssc.SscLog;
using Ssc.SscPool;

namespace Ssc.SscTemplate {

    public class PoolAllocator<T> where T : IMemoryable{

        private static ObjectPool<T> _objectPools;
        private static readonly Logger Logger = LogManager.GetLogger<PoolAllocator<T>>(LogType.Middle);

        public static void SetPool(ObjectActivator<T> objectGenerator) {
            if (_objectPools != null) {
                return;
            }
            _objectPools = new ObjectPool<T>(objectGenerator);
        }

        public static T GetObject(params object[] args) {
            if (_objectPools == null) {
                Logger.Error($"没有调用{nameof(SetPool)}初始划{typeof(T).Name}！");
                return default;
            }

            var obj = _objectPools.GetObject(args);
            obj.Assign();
            return obj;
        }

        public static void Recycle(T value) {
            if (value == null) {
                Logger.Error($"{nameof(value)}值为空！");
                return;
            }
            
            value.Recycle();
            _objectPools?.PutObject(value);
        }
    }
}