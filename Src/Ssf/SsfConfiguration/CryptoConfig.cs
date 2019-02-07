using System;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Ssc.SscExtension;
using Ssc.SscStream;
using Ssf.Ssf;

namespace Ssf.SsfConfiguration {
    public class CryptoConfig {
        public CryptoConfig() {
            Encryptor = DefaultEncryptor;
            Decryptor = DefaultDecryptor;
        }
        [JsonIgnore]
        public Encryptor Encryptor { get; set; }
        [JsonIgnore]
        public Decryptor Decryptor { get; set; }

        public bool Switch { get; set; } = true;
        public int CryptonKeyLength { get; set; } = 8;
        public byte[] Salt { get; set; } = "5x685$%(*_@#$%".ToBytes();
        public int RsaKeySize { get; set; } = 512;

        private void DefaultEncryptor(IWriteStream writeStream, string sharedSecret) {
            if (!Switch) {
                return;
            }
            using (var aesAlg = new RijndaelManaged()) {
                var key = new Rfc2898DeriveBytes(sharedSecret, Ssfi.CryptoConfig.Salt);
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                var byteFragment = writeStream.ToByteFragment();

                using (var resultStream = new MemoryStream()) {
                    using (var csEncrypt = new CryptoStream(resultStream, encryptor, CryptoStreamMode.Write)) {
                        csEncrypt.Write(byteFragment.Buffer, byteFragment.Offset, byteFragment.Count);
                    }

                    writeStream.Reset();
                    writeStream.ShiftLeft(aesAlg.IV);
                    writeStream.ShiftLeft((ushort) aesAlg.IV.Length);
                    writeStream.ShiftRight((ushort) byteFragment.Count);
                    writeStream.ShiftRight(resultStream.ToArray());
                }
            }
        }

        private void DefaultDecryptor(IReadStream readStream, string sharedSecret) {
            if (!Switch) {
                return;
            }

            using (var aesAlg = new RijndaelManaged()) {
                var key = new Rfc2898DeriveBytes(sharedSecret, Ssfi.CryptoConfig.Salt);
                aesAlg.Padding = PaddingMode.None;

                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                var ivLength = readStream.ShiftRight<ushort>();
                var iv = readStream.ShiftRight(ivLength);
                var bytes = new byte[iv.Count];
                Buffer.BlockCopy(iv.Buffer, iv.Offset, bytes, 0, iv.Count);
                aesAlg.IV = bytes;

                var length = readStream.ShiftRight<ushort>();
                var byteFragment = readStream.ToByteFragment();

                using (var msDecrypt = new MemoryStream(
                    byteFragment.Buffer, byteFragment.Offset, byteFragment.Count)) {
                    var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                        csDecrypt.Read(byteFragment.Buffer, byteFragment.Offset, length);
                    }
                }
            }
        }

        #region Password hashing

        public int SALT_BYTE_SIZE { get; set; } = 24;
        public int HASH_BYTE_SIZE { get; set; } = 24;
        public int PBKDF2_ITERATIONS { get; set; } = 1000;

        public int ITERATION_INDEX { get; set; } = 0;
        public int SALT_INDEX { get; set; } = 1;
        public int PBKDF2_INDEX { get; set; } = 2;

        #endregion Password hashing
    }
}