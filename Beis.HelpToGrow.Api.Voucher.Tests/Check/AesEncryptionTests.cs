using NUnit.Framework;

namespace Beis.HelpToGrow.Api.Voucher.Tests.Check
{
    [TestFixture]
    public class AesEncryptionTests
    {
        private IEncryptionService _encryptionService;
        private string _salt;
        private int _passwordIterations;
        private string _initialVector;
        private int _keySize;
        private string PlainText = "plainText";
        private string Password = "password";
        private string EncryptedString = "o53lyNThysHnSssdRmEzjQ=="; //"ujd9uSDwqMTyQ8IzeEJtGw=="; // 

        [SetUp]
        public void Setup()
        {
            _salt = "Unittestsa";
            _passwordIterations = 2;
            _initialVector = "ABCab14x*hjk01wF";
            _keySize = 256;
            IOptions<EncryptionSettings> _options = Options.Create<EncryptionSettings>(new EncryptionSettings 
                { 
                    VoucherEncryptionSalt = _salt, 
                    VoucherEncryptionIteration = _passwordIterations, 
                    VoucherEncryptionInitialVector = _initialVector, 
                    VoucherEncryptionKeySize = _keySize 
                });
            _encryptionService = new AesEncryption(_options);
        }
        
        [Test]
        public void CallingEncryptReturnsValidEncodedString()
        {
            SetUpDefaultValues();
            var response = _encryptionService.Encrypt(PlainText, Password);
            Assert.AreEqual(EncryptedString, response);
        }

        [Test]
        public void CallingEncryptWithEmptyPlainTextReturnsEmptyString()
        {
            SetUpDefaultValues();
            PlainText = "";
            var response = _encryptionService.Encrypt(PlainText, Password);
            Assert.AreEqual(string.Empty, response);
        }

        [Test]
        public void CallingEncryptWithNullPlainTextReturnsEmptyString()
        {
            SetUpDefaultValues();
            PlainText = null;
            var response = _encryptionService.Encrypt(PlainText, Password);
            Assert.AreEqual(string.Empty, response);
        }

        [Test]
        public void CallingDecryptReturnsValidPlainTextString()
        {
            SetUpDefaultValues();
            var response = _encryptionService.Decrypt(EncryptedString, Password);
            Assert.AreEqual(PlainText, response);
        }

        [Test]
        public void CallingDecryptWithEmptyEncryptedStringReturnsEmptyString()
        {
            SetUpDefaultValues();
            EncryptedString = "";
            var response = _encryptionService.Decrypt(EncryptedString, Password);
            Assert.AreEqual(string.Empty, response);
        }

        [Test]
        public void CallingDecryptWithNullEncryptedStringReturnsEmptyString()
        {
            SetUpDefaultValues();
            EncryptedString = null;
            var response = _encryptionService.Decrypt(EncryptedString, Password);
            Assert.AreEqual(string.Empty, response);
        }

        private void SetUpDefaultValues()
        {
            PlainText = "plainText";
            Password = "password";
            EncryptedString = "o53lyNThysHnSssdRmEzjQ==";
        }
    }
}