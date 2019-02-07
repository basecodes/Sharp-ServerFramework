using Ssc.SscStream;

namespace Ssf.SsfNetwork.Sockets {
    public struct Recorder {
        private ulong compareValue;
        private int _count;
        public int Count {
            get {
                return _count;
            }
            set {
                _count = value;
                for (int i = 0; i < value; i++) {
                    compareValue += (ulong)(1 << i);
                }
            }
        }
        public ulong Flag { get; set; }
        public IReadStream ReadStream { get; set; }

        public bool IsComplete() {
            return compareValue == Flag;
        }
    }
}
