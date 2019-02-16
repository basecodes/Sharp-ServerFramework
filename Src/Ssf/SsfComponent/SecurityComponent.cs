using Ssc.SscTemplate;
using Ssm.SsmComponent;

namespace Ssf.SsfComponent {
    public class SecurityComponent : PoolAllocator<ISecurityComponent>,ISecurityComponent {
        public string AesKey { get; set; }
        public byte[] Encryptkey { get; set; }

        public void Assign() {
        }

        public void Dispose() {
            Recycle(this);
        }

        public void Recycle() {
            AesKey = null;
            Encryptkey = null;
        }
    }
}