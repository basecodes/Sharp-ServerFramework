using System;
using System.Linq;
using Ssc.Ssc;
using Ssc.SscFactory;
using Ssc.SscLog;
using Ssc.SscPool;

namespace Ssc.SscTemplate {
    public class PoolAllocator2<T> where T : IRecyclable,IAssignable,new() {

        private static ObjectPool<T> _objectPools = new ObjectPool<T>((args) =>
        ObjectFactory.GetActivator<T>(typeof(T).GetConstructors().First())());

        private static readonly Logger Logger = LogManager.GetLogger<PoolAllocator2<T>>(LogType.Middle);

        public virtual void Recycle() {
            if (this is T value) {
                _objectPools?.PutObject(value);
            }
        }

        public static T GetObject(params object[] args) {
            return _objectPools.GetObject(args);
        }

        public static void Recycle(T value) {
            if (value == null) throw new ArgumentNullException(nameof(value));
            value.Recycle();
            _objectPools?.PutObject(value);
        }
    }
}
