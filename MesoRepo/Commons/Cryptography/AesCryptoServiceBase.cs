using System;
using System.IO;
using System.Security.Cryptography;
using Commons.Text;
using Serilog;

namespace Commons.Cryptography {
    public class AesCryptoServiceBase {
        /// <summary>
        ///     Legal AES Key Sizes are 128, 192, 256 bit.
        ///     DefAesKeySize must be key size divided by 8.
        ///     Uses the maximum legal key size.
        /// </summary>
        public const int DefAesKeySize = 32;

        /// <summary>
        ///     Only legal AES block size is 128 bits. Value must be blocksize devided by 8.
        /// </summary>
        public const int DefAesIvSize = 16;

        private readonly AesCryptoServiceProvider aes;

        protected AesCryptoServiceBase(AesCspParameters aesCspParameters) {
            if (null == aesCspParameters) {
                throw new ArgumentNullException(nameof(aesCspParameters));
            }

            aes = new AesCryptoServiceProvider {
                Key = HexTk.UnHex(aesCspParameters.AesKey),
                IV = HexTk.UnHex(aesCspParameters.AesIv)
            };
        }

        protected string EncryptString<T>(T source) {
            string encrypted = null;

            if (null != source) {
                using var msEncrypt = new MemoryStream();
                var encryptor = aes.CreateEncryptor();
                using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

                // socpe of sw here is important! Encoding does not happen before sw is closed - writing is finished!
                using (var swEncrypt = new StreamWriter(csEncrypt)) {
                    swEncrypt.Write(source.ToString());
                }

                var encryptedBytes = msEncrypt.ToArray();
                encrypted = HexTk.HexDump(encryptedBytes);
            }

            return encrypted;
        }

        /// <summary>
        ///     inverts the encryption of a string - works if the same AES key has been used for encryption.
        ///     Notice for programmers: Each decryption needs a fresh decryptor, reuse of decryptor throws ugly exceptions.
        /// </summary>
        /// <param name="encryptedText"></param>
        /// <returns></returns>
        protected string DecryptString(string encryptedText) {
            var decrypted = string.Empty;

            try {
                var trimmed = encryptedText.ParseText();
                if (!string.IsNullOrEmpty(trimmed)) {
                    var unHexed = HexTk.UnHex(trimmed);
                    using var msDecrypt = new MemoryStream(unHexed);
                    // must get a fresh Decryptor for each decryption
                    var decryptor = aes.CreateDecryptor();
                    using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                    using var srDecrypt = new StreamReader(csDecrypt);
                    decrypted = srDecrypt.ReadToEnd();
                }
            } catch (Exception exception) {
                Log.Warning(exception, AesResources.Decryption_Failed, encryptedText);
            }

            return decrypted;
        }
    }
}