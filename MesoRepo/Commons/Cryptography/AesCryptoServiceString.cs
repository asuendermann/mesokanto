namespace Commons.Cryptography {
    public class AesCryptoServiceString : AesCryptoServiceBase, ICryptoService<string> {
        public AesCryptoServiceString(AesCspParameters aesCspParameters) : base(aesCspParameters) {
        }

        public string Encrypt(string decrypted) {
            return EncryptString(decrypted);
        }

        public string Decrypt(string encrypted) {
            return DecryptString(encrypted);
        }
    }
}