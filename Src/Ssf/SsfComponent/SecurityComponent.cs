using Ssc.SscTemplate;
using Ssm.SsmComponent;

namespace Ssf.SsfComponent {
    public class SecurityComponent : PoolAllocator<ISecurityComponent>,ISecurityComponent {
        public string AesKey { get; set; }
        public byte[] Encryptkey { get; set; }

        public void Dispose() {
            Recycle();
        }

        public override void Recycle() {
            base.Recycle();

            AesKey = null;
            Encryptkey = null;
        }
    }
}