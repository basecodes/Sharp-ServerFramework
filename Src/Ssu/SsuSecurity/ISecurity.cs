using Ssc.SscStream;

namespace Ssu.SsuSecurity {
    internal class Keys {
        public string PrivateKey;
        public string PublicKey;
    }

    internal interface ISecurity {
        bool IsEncrypt { get; set; }

        byte[] EncryptAesKey(string key, string publicKey);
        byte[] DecryptAesKey(byte[] bytes, Keys keys);

        Keys CreateAesKey();

        void EncryptAES(IWriteStream writeStream, string sharedSecret);
        void DecryptAES(IReadStream readStream, string sharedSecret);

        string EncryptStringAES(string plainText, string sharedSecret);
        string DecryptStringAES(string cipherText, string sharedSecret);

        string CreateHash(string password);

        bool ValidatePassword(string password, string correctHash);
    }
}