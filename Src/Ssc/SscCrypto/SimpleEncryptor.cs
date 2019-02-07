namespace Ssc.SscCrypto {
    public class SimpleEncryptor : BaseCrypto {
        public SimpleEncryptor(byte[] key) : base(key) {
        }

        protected override byte DoByte(byte b) {
            var b2 = Multiply257((byte) (b + _currentKey), _currentKey);
            _currentKey = Multiply257((byte) (b + b2), _currentKey);
            return b2;
        }
    }
}