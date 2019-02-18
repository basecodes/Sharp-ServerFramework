using Ssc;
using Ssc.Ssc;
using Ssc.SscFactory;
using Sss.SssModule;
using Sss.SssSerialization.Python;
using System;
using System.Linq;

namespace Sss.SssScripts.Python {
    public class PythonProxy {

        public static ClassWrapper<IPythonPacket> CreatePacket(string interfaceName, dynamic instance, PythonHelper pythonHelper) {
            if (string.IsNullOrEmpty(interfaceName)) {
                throw new ArgumentNullException(nameof(interfaceName));
            }

            if (instance == null) {
                throw new ArgumentNullException(nameof(instance));
            }

            if (pythonHelper == null) {
                throw new ArgumentNullException(nameof(pythonHelper));
            }

            var luaPacket = ObjectFactory.GetActivator<IPythonPacket>(
                typeof(PythonPacket).GetConstructors().First())(interfaceName, instance, pythonHelper);
            return new ClassWrapper<IPythonPacket>(luaPacket);
        }

        public static PythonController CreateControler(dynamic instance) {
            if (instance == null) {
                throw new ArgumentNullException(nameof(instance));
            }

            return ObjectFactory.GetActivator<PythonController>(
                typeof(PythonController).GetConstructors().First())(instance);
        }

        public static PythonModule CreateModule(dynamic instance, PythonHelper pythonHelper) {
            if (instance == null) {
                throw new ArgumentNullException(nameof(instance));
            }

            if (pythonHelper == null) {
                throw new ArgumentNullException(nameof(pythonHelper));
            }

            return ObjectFactory.GetActivator<PythonModule>(
                typeof(PythonModule).GetConstructors().First())(instance, pythonHelper);
        }
    }
}
