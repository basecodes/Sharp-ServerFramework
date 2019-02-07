namespace Ssc.SscCrypto {
    public class SimpleDecryptor : BaseCrypto {
        public SimpleDecryptor(byte[] key) : base(key) {
        }

        protected override byte DoByte(byte b) {
            var b2 = (byte) (Multiply257(b, Complement257(_currentKey)) - _currentKey);
            _currentKey = Multiply257((byte) (b + b2), _currentKey);
            return b2;
        }
    }
}