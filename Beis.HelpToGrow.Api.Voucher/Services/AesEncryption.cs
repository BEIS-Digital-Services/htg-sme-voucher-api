using System.Security.Cryptography;

namespace Beis.HelpToGrow.Api.Voucher.Services
{
    public class AesEncryption : IEncryptionService
    {
        private readonly string _salt;
        private readonly int _passwordIterations;
        private readonly string _initialVector;
        private readonly int _keySize;

        public AesEncryption(IOptions<EncryptionSettings> options)
        {
            _salt = options.Value.VOUCHER_ENCRYPTION_SALT;
            _passwordIterations = options.Value.VOUCHER_ENCRYPTION_ITERATION;
            _initialVector = options.Value.VOUCHER_ENCRYPTION_INITIAL_VECTOR;
            _keySize = options.Value.VOUCHER_ENCRYPTION_KEY_SIZE;
        }

        public string Encrypt(string plainText, string password)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return "";
            }

            byte[] initialVectorBytes = Encoding.ASCII.GetBytes(_initialVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(_salt);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            Rfc2898DeriveBytes derivedPassword = new Rfc2898DeriveBytes(password, saltValueBytes, _passwordIterations);
            byte[] keyBytes = derivedPassword.GetBytes(_keySize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            byte[] cipherTextBytes;
            using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initialVectorBytes))
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        cryptoStream.FlushFinalBlock();
                        cipherTextBytes = memStream.ToArray();
                        memStream.Close();
                        cryptoStream.Close();
                    }
                }
            }
            symmetricKey.Clear();

            var encodedString = Convert.ToBase64String(cipherTextBytes);
            return encodedString.Replace('+', '-').Replace('/', '_');
        }

        public string Decrypt(string cipherText, string password)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return "";
            }

            cipherText = cipherText.Replace('-', '+').Replace('_', '/');

            byte[] initialVectorBytes = Encoding.ASCII.GetBytes(_initialVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(_salt);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            var derivedPassword = new Rfc2898DeriveBytes(password, saltValueBytes, _passwordIterations);
            byte[] keyBytes = derivedPassword.GetBytes(_keySize / 8);
            var symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            var plainTextBytes = new byte[cipherTextBytes.Length];
            int byteCount;
            using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, initialVectorBytes))
            {
                using (var memStream = new MemoryStream(cipherTextBytes))
                {
                    using (var cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read))
                    {

                        byteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                        memStream.Close();
                        cryptoStream.Close();
                    }
                }
            }
            symmetricKey.Clear();
            return Encoding.UTF8.GetString(plainTextBytes, 0, byteCount);
        }
    }
}