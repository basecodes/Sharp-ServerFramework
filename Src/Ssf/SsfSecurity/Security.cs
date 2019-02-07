using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Serialization;
using Ssc.SscExtension;
using Ssc.SscLog;
using Ssc.SscStream;

namespace Ssf.SsfSecurity {
    internal class Security : ISecurity {
        private static readonly Logger Logger = LogManager.GetLogger<Security>(LogType.Middle);

        public byte[] EncryptAesKey(string key, string publicKey) {
            if (string.IsNullOrEmpty(publicKey)) {
                Logger.Warn($"{nameof(publicKey)}为 null！");
                return null;
            }

            using (var csp = new RSACryptoServiceProvider()) {
                csp.ImportParameters(GetRSAParameters(publicKey));
                var encryptedAes = csp.Encrypt(key.ToBytes(), false);

                return encryptedAes;
            }
        }

        public byte[] DecryptAesKey(byte[] bytes, Keys keys) {
            if (keys == null) {
                Logger.Warn($"{nameof(keys)}为 null！");
                return null;
            }

            using (var csp = new RSACryptoServiceProvider()) {
                csp.ImportParameters(GetRSAParameters(keys.PublicKey));
                csp.ImportParameters(GetRSAParameters(keys.PrivateKey));

                var encryptedAes = csp.Decrypt(bytes, false);

                return encryptedAes;
            }
        }

        public Keys CreateAesKey() {
            using (var csp = new RSACryptoServiceProvider(Ssfi.CryptoConfig.RsaKeySize)) {
                return new Keys {
                    PublicKey = GetKeyString(csp.ExportParameters(false)),
                    PrivateKey = GetKeyString(csp.ExportParameters(true))
                };
            }
        }

        public void EncryptAES(IWriteStream writeStream, string sharedSecret) {
            Ssfi.CryptoConfig.Encryptor(writeStream, sharedSecret);
        }

        public void DecryptAES(IReadStream readStream, string sharedSecret) {
            Ssfi.CryptoConfig.Decryptor(readStream, sharedSecret);
        }

        public string EncryptStringAES(string plainText, string sharedSecret) {
            string outStr = null;
            RijndaelManaged aesAlg = null;

            try {
                var key = new Rfc2898DeriveBytes(sharedSecret, Ssfi.CryptoConfig.Salt);

                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream()) {
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
                        using (var swEncrypt = new StreamWriter(csEncrypt)) {
                            swEncrypt.Write(plainText);
                        }
                    }

                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            } finally {
                aesAlg?.Clear();
            }

            return outStr;
        }

        public string DecryptStringAES(string cipherText, string sharedSecret) {
            RijndaelManaged aesAlg = null;

            string plaintext = null;

            try {
                var key = new Rfc2898DeriveBytes(sharedSecret, Ssfi.CryptoConfig.Salt);

                var bytes = Convert.FromBase64String(cipherText);
                using (var msDecrypt = new MemoryStream(bytes)) {
                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    aesAlg.IV = ReadByteArray(msDecrypt);

                    var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                        using (var srDecrypt = new StreamReader(csDecrypt)) {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            } finally {
                if (aesAlg != null) aesAlg.Clear();
            }

            return plaintext;
        }

        public string CreateHash(string password) {
            var csprng = new RNGCryptoServiceProvider();
            var salt = new byte[Ssfi.CryptoConfig.SALT_BYTE_SIZE];
            csprng.GetBytes(salt);

            var hash = PBKDF2(password, salt, Ssfi.CryptoConfig.PBKDF2_ITERATIONS,
                Ssfi.CryptoConfig.HASH_BYTE_SIZE);
            return Ssfi.CryptoConfig.PBKDF2_ITERATIONS + ":" +
                   Convert.ToBase64String(salt) + ":" +
                   Convert.ToBase64String(hash);
        }

        public bool ValidatePassword(string password, string correctHash) {
            var split = correctHash.Split(':');
            var iterations = int.Parse(split[Ssfi.CryptoConfig.ITERATION_INDEX]);
            var salt = Convert.FromBase64String(split[Ssfi.CryptoConfig.SALT_INDEX]);
            var hash = Convert.FromBase64String(split[Ssfi.CryptoConfig.PBKDF2_INDEX]);

            var testHash = PBKDF2(password, salt, iterations, hash.Length);
            return SlowEquals(hash, testHash);
        }

        private RSAParameters GetRSAParameters(string key) {
            var sr = new StringReader(key);
            var xs = new XmlSerializer(typeof(RSAParameters));
            return (RSAParameters) xs.Deserialize(sr);
        }

        private string GetKeyString(RSAParameters rsaParameters) {
            var sw = new StringWriter();
            var xs = new XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, rsaParameters);
            return sw.ToString();
        }

        private static byte[] ReadByteArray(Stream s) {
            var rawLength = new byte[sizeof(ushort)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
                throw new SystemException("Stream did not contain properly formatted byte array");

            var buffer = new byte[rawLength.ToValue<ushort>()];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
                throw new SystemException("Did not read byte array properly");

            return buffer;
        }

        private static bool SlowEquals(byte[] a, byte[] b) {
            var diff = (uint) a.Length ^ (uint) b.Length;
            for (var i = 0; i < a.Length && i < b.Length; i++) diff |= (uint) (a[i] ^ b[i]);
            return diff == 0;
        }

        private static byte[] PBKDF2(string password, byte[] salt, int iterations, int outputBytes) {
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt);
            pbkdf2.IterationCount = iterations;
            return pbkdf2.GetBytes(outputBytes);
        }

        public static byte[] Hash(string value, byte[] salt) {
            return Hash(value.ToBytes(), salt);
        }

        public static byte[] Hash(byte[] value, byte[] salt) {
            var saltedValue = value.Concat(salt).ToArray();
            return new SHA256Managed().ComputeHash(saltedValue);
        }
    }
}