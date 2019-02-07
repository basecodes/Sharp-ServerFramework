using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Serialization;
using Ssc.SscLog;
using Ssc.SscStream;
using Ssc.SscExtension;

namespace Ssu.SsuSecurity{
    internal class Security : ISecurity {
        private static readonly Logger Logger = LogManager.GetLogger<Security>(LogType.Middle);
        private readonly byte[] _salt = "5x685$%(*_@#$%".ToBytes();

        public int RsaKeySize = 512;

        public bool IsEncrypt { get; set; } = true;

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
            using (var csp = new RSACryptoServiceProvider(RsaKeySize)) {
                return new Keys {
                    PublicKey = GetKeyString(csp.ExportParameters(false)),
                    PrivateKey = GetKeyString(csp.ExportParameters(true))
                };
            }
        }

        public void EncryptAES(IWriteStream writeStream, string sharedSecret) {
            if (!IsEncrypt) return;
            using (var aesAlg = new RijndaelManaged()) {
                var key = new Rfc2898DeriveBytes(sharedSecret, _salt);
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                var byteFragment = writeStream.ToByteFragment();

                using (var resultStream = new MemoryStream()) {
                    using (var csEncrypt = new CryptoStream(resultStream, encryptor, CryptoStreamMode.Write)) {
                        csEncrypt.Write(byteFragment.Buffer, byteFragment.Offset, byteFragment.Count);
                    }

                    writeStream.Reset();
                    writeStream.ShiftLeft(aesAlg.IV);
                    var ivLength = (ushort) aesAlg.IV.Length;
                    writeStream.ShiftLeft(ivLength);
                    writeStream.ShiftRight((ushort) byteFragment.Count);
                    var bytes = resultStream.ToArray();
                    writeStream.ShiftRight(bytes);
                }
            }
        }

        public void DecryptAES(IReadStream readStream, string sharedSecret) {
            if (!IsEncrypt) return;
            using (var aesAlg = new RijndaelManaged()) {
                var key = new Rfc2898DeriveBytes(sharedSecret, _salt);
                aesAlg.Padding = PaddingMode.None;

                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                var iv = readStream.ShiftRight(readStream.ShiftRight<ushort>());
                var bytes = new byte[iv.Count];
                Buffer.BlockCopy(iv.Buffer, iv.Offset, bytes, 0, iv.Count);
                aesAlg.IV = bytes;

                var length = readStream.ShiftRight<ushort>();
                var byteFragment = readStream.ToByteFragment();

                using (var msDecrypt = new MemoryStream(byteFragment.Buffer, byteFragment.Offset, byteFragment.Count)) {
                    var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                        csDecrypt.Read(byteFragment.Buffer, byteFragment.Offset, length);
                    }
                }
            }
        }

        public string EncryptStringAES(string plainText, string sharedSecret) {
            string outStr = null;
            RijndaelManaged aesAlg = null;

            try {
                var key = new Rfc2898DeriveBytes(sharedSecret, _salt);

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
                var key = new Rfc2898DeriveBytes(sharedSecret, _salt);

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
            var salt = new byte[SALT_BYTE_SIZE];
            csprng.GetBytes(salt);

            var hash = PBKDF2(password, salt, PBKDF2_ITERATIONS, HASH_BYTE_SIZE);
            return PBKDF2_ITERATIONS + ":" +
                   Convert.ToBase64String(salt) + ":" +
                   Convert.ToBase64String(hash);
        }

        public bool ValidatePassword(string password, string correctHash) {
            var split = correctHash.Split(':');
            var iterations = int.Parse(split[ITERATION_INDEX]);
            var salt = Convert.FromBase64String(split[SALT_INDEX]);
            var hash = Convert.FromBase64String(split[PBKDF2_INDEX]);

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

        #region Password hashing

        public const int SALT_BYTE_SIZE = 24;
        public const int HASH_BYTE_SIZE = 24;
        public const int PBKDF2_ITERATIONS = 1000;

        public const int ITERATION_INDEX = 0;
        public const int SALT_INDEX = 1;
        public const int PBKDF2_INDEX = 2;

        #endregion Password hashing
    }
}