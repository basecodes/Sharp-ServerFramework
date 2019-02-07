using System.Linq;
using Ssc.SscFactory;
using Ssc.SscTemplate;
using Sss.SssComponent;

namespace Sss.SssManager {
    internal class PoolManager {

        public PoolManager() {
            PoolAllocator<LuaPeerComponent>.SetPool(
        args => ObjectFactory.GetActivator<LuaPeerComponent>(
            typeof(LuaPeerComponent).GetConstructors().First())());
        }
    }
}
