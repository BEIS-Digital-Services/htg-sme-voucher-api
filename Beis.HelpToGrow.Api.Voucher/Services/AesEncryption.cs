using System.Security.Cryptography;

namespace Beis.HelpToGrow.Api.Voucher.Services
{
    public class AesEncryption : IEncryptionService
    {
        private readonly EncryptionSettings _encryptionOptions;

        public AesEncryption(IOptions<EncryptionSettings> encryptionOptions)
        {
            _encryptionOptions = encryptionOptions.Value;
        }

        public string Encrypt(string plainText, string password)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return "";
            }

            byte[] initialVectorBytes = Encoding.ASCII.GetBytes(_encryptionOptions.VoucherEncryptionInitialVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(_encryptionOptions.VoucherEncryptionSalt);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            Rfc2898DeriveBytes derivedPassword = new Rfc2898DeriveBytes(password, saltValueBytes, _encryptionOptions.VoucherEncryptionIteration);
            byte[] keyBytes = derivedPassword.GetBytes(_encryptionOptions.VoucherEncryptionKeySize / 8);
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

            byte[] initialVectorBytes = Encoding.ASCII.GetBytes(_encryptionOptions.VoucherEncryptionInitialVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(_encryptionOptions.VoucherEncryptionSalt);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            var derivedPassword = new Rfc2898DeriveBytes(password, saltValueBytes, _encryptionOptions.VoucherEncryptionIteration);
            byte[] keyBytes = derivedPassword.GetBytes(_encryptionOptions.VoucherEncryptionKeySize / 8);
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