using Serilog;

namespace Commons.Cryptography {
    public class AesCryptoServiceInt : AesCryptoServiceBase, ICryptoService<int> {
        public AesCryptoServiceInt(AesCspParameters aesCspParameters) : base(aesCspParameters) {
        }

        public string Encrypt(int decrypted) {
            return EncryptString(decrypted);
        }

        public int Decrypt(string encrypted) {
            var intString = DecryptString(encrypted);
            if (!int.TryParse(intString, out var intValue)) {
                Log.Warning(AesResources.Decryption_Failed, encrypted);
                return 0;
            }

            return intValue;
        }
    }
}