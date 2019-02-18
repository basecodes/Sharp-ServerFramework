using Ssc.SscSerialization;
using Ssc.SscTemplate;
using Sss.SssScripts.Python;
using System;

namespace Sss.SssSerialization.Python {

    public interface IPythonPacket : ISerializablePacket {
        dynamic Instance { get; }
    }

    public class PythonPacket : ScriptPoolAllocator<IPythonPacket>, IPythonPacket {
        public override string TypeName { get; }
        public dynamic Instance { get; }
        private readonly PythonHelper _pythonHelper;

        public PythonPacket(string interfaceName,dynamic instance,PythonHelper pythonHelper) {
            TypeName = interfaceName ?? throw new ArgumentNullException(nameof(interfaceName));
            Instance = instance ?? throw new ArgumentNullException(nameof(instance));
            _pythonHelper = pythonHelper ?? throw new ArgumentNullException(nameof(pythonHelper));
        }
        public void Assign() {
            Instance.Assign();
        }

        public void Recycle() {
            Instance.Recycle();
        }

        public void FromBinaryReader(IEndianBinaryReader reader) {
            Instance.FromBinaryReader(reader);
        }

        public void ToBinaryWriter(IEndianBinaryWriter writer) {
            Instance.ToBinaryWriter(writer);
        }
    }
}
