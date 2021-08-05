namespace Commons.Cryptography {
    public interface ICryptoService<T> {
        string Encrypt(T decrypted);


        T Decrypt(string encrypted);

    }
}