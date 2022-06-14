using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;


namespace Beis.HelpToGrow.Api.Voucher.Tests.Check
{
    [TestFixture]
    public class VoucherCheckServiceTests
    {
        private VoucherCheckService _voucherCheckService;
        private Mock<IEncryptionService> _encryptionService;
        private Mock<ITokenRepository> _tokenRepository;
        private Mock<IProductRepository> _productRepository;
        private Mock<IVendorCompanyRepository> _vendorCompanyRepository;
        private Mock<IEnterpriseRepository> _enterpriseRepository;
        private Mock<ILogger<VoucherCheckService>> _logger;
        private Mock<IVendorAPICallStatusServices> _vendorApiCallStatusService;
        vendor_api_call_status vendorApiCallStatus = new vendor_api_call_status
        {
            error_code = "200"
        };
             
        [SetUp]

        public void Setup()
        {
            _encryptionService = new Mock<IEncryptionService>();
            _tokenRepository = new Mock<ITokenRepository>();
            _productRepository = new Mock<IProductRepository>();
            _vendorCompanyRepository = new Mock<IVendorCompanyRepository>();
            _enterpriseRepository = new Mock<IEnterpriseRepository>();
            _logger = new Mock<ILogger<VoucherCheckService>>();
            _vendorApiCallStatusService = new Mock<IVendorAPICallStatusServices>();
            _vendorApiCallStatusService.Setup(x => x.CreateLogRequestDetails(It.IsAny<VoucherCheckRequest>())).Returns( (VoucherCheckRequest r) => vendorApiCallStatus);
                
               

            _voucherCheckService = new VoucherCheckService(_logger.Object,
                                                                               _encryptionService.Object,
                                                                               _tokenRepository.Object,
                                                                               _productRepository.Object,
                                                                               _vendorCompanyRepository.Object,
                                                                               _enterpriseRepository.Object,
                                                                               _vendorApiCallStatusService.Object);
        }

        [Test]

        public async Task VoucherCheckServiceGetVoucherResponseHappyPathTests()
        {
            var voucherRequest = setuptestData("12345");
            
            VoucherCheckResponse voucherResponse = await _voucherCheckService.GetVoucherResponse(voucherRequest);
            
            Assert.NotNull(voucherResponse);
            Assert.AreEqual("OK", voucherResponse.Status);
            Assert.AreEqual(voucherRequest.VoucherCode, voucherResponse.VoucherCode);
            Assert.AreEqual("GBP", voucherResponse.Currency);
            Assert.AreEqual("abcdef", voucherResponse.ProductName);
            Assert.AreEqual("abcdefgh", voucherResponse.LicenceTo);
            Assert.AreEqual(0, voucherResponse.ErrorCode);
        }

        [Test]
        public async Task VoucherCheckServiceGetVoucherCheckResponseHappyPathTests()
        {
            var voucherRequest = setuptestData("12345");

            VoucherCheckResponse voucherResponse = await _voucherCheckService.GetVoucherResponse(voucherRequest);

            Assert.NotNull(voucherResponse);
            Assert.AreEqual("OK", voucherResponse.Status);
            Assert.AreEqual("Successful check - proceed with Voucher", voucherResponse.Message.Trim());
            Assert.AreEqual(voucherRequest.VoucherCode, voucherResponse.VoucherCode);
            Assert.AreEqual("GBP", voucherResponse.Currency);
            Assert.AreEqual("abcdef", voucherResponse.ProductName);
            Assert.AreEqual("abcdefgh", voucherResponse.LicenceTo);
            Assert.AreEqual(0, voucherResponse.ErrorCode);
        }

        [Test]
        public async Task VoucherCheckServiceGetVoucherResponseNegativePathTests()
        {
            var voucherRequest = setuptestData("99999");

            VoucherCheckResponse voucherResponse = await _voucherCheckService.GetVoucherResponse(voucherRequest);
            
            Assert.AreEqual("ERROR", voucherResponse.Status);
            Assert.AreEqual(10, voucherResponse.ErrorCode);
            Assert.AreEqual("Unknown token Invalid vendor details", voucherResponse.Message.Trim());
            Assert.AreEqual(voucherRequest.VoucherCode, voucherResponse.VoucherCode);
        }

        [Test]
        public async Task VoucherCheckServiceGetVoucherWithEmptyTokenResponseNegativePathTests()
        {
            var voucherRequest = setuptestData("12345");

            _tokenRepository.Setup(x => x.GetTokenByTokenCode(It.IsAny<string>())).Returns((token)null);

            VoucherCheckResponse voucherResponse = await _voucherCheckService.GetVoucherResponse(voucherRequest);

            Assert.AreEqual("ERROR", voucherResponse.Status);
            Assert.AreEqual(10, voucherResponse.ErrorCode);
            Assert.AreEqual("Unknown token Unknown token", voucherResponse.Message.Trim());
            Assert.AreEqual(voucherRequest.VoucherCode, voucherResponse.VoucherCode);
        }

        [Test]
        public async Task VoucherCheckServiceGetVoucherWithEmptyVendorCompanyResponseNegativePathTests()
        {
            var voucherRequest = setuptestData("12345");

            _vendorCompanyRepository.Setup(x => x.GetVendorCompanyByRegistration(It.IsAny<string>()))
                .Returns((vendor_company)null);
            VoucherCheckResponse voucherResponse = await _voucherCheckService.GetVoucherResponse(voucherRequest);

            Assert.AreEqual("ERROR", voucherResponse.Status);
            Assert.AreEqual(10, voucherResponse.ErrorCode);
            Assert.AreEqual("Unknown token Invalid Vendor company", voucherResponse.Message.Trim());
            Assert.AreEqual(voucherRequest.VoucherCode, voucherResponse.VoucherCode);
        }

        [Test]
        public async Task VoucherCheckServiceGetVoucherWithExpiredTokenResponseNegativePathTests()
        {
            var voucherRequest = setuptestData("12345");

            var invalidToken = _tokenRepository.Object.GetTokenByTokenCode(It.IsAny<string>());
            invalidToken.token_expiry = DateTime.Now.AddSeconds(-1);
            _tokenRepository.Setup(x => x.GetTokenByTokenCode(It.IsAny<string>())).Returns(invalidToken);

            VoucherCheckResponse voucherResponse = await _voucherCheckService.GetVoucherResponse(voucherRequest);

            Assert.AreEqual("ERROR", voucherResponse.Status);
            Assert.AreEqual(20, voucherResponse.ErrorCode);
            Assert.AreEqual("Expired Token", voucherResponse.Message.Trim());
            Assert.AreEqual(voucherRequest.VoucherCode, voucherResponse.VoucherCode);
        }

        [Test]
        public async Task VoucherCheckServiceGetVoucherWithNoBalanceTokenResponseNegativePathTests()
        {
            var voucherRequest = setuptestData("12345");

            var invalidToken = _tokenRepository.Object.GetTokenByTokenCode(It.IsAny<string>());
            invalidToken.token_balance = 0;
            _tokenRepository.Setup(x => x.GetTokenByTokenCode(It.IsAny<string>())).Returns(invalidToken);

            VoucherCheckResponse voucherResponse = await _voucherCheckService.GetVoucherResponse(voucherRequest);

            Assert.AreEqual("ERROR", voucherResponse.Status);
            Assert.AreEqual(30, voucherResponse.ErrorCode);
            Assert.AreEqual("No Balance", voucherResponse.Message.Trim());
            Assert.AreEqual(voucherRequest.VoucherCode, voucherResponse.VoucherCode);
        }

        [Test]
        public async Task VoucherCheckServiceGetVoucherWithLockedResponseNegativePathTests()
        {
            var voucherRequest = setuptestData("12345");

            var invalidToken = _tokenRepository.Object.GetTokenByTokenCode(It.IsAny<string>());
            invalidToken.authorisation_code = null;
            _tokenRepository.Setup(x => x.GetTokenByTokenCode(It.IsAny<string>())).Returns(invalidToken);

            VoucherCheckResponse voucherResponse = await _voucherCheckService.GetVoucherResponse(voucherRequest);

            Assert.AreEqual("ERROR", voucherResponse.Status);
            Assert.AreEqual(40, voucherResponse.ErrorCode);
            Assert.AreEqual("Locked", voucherResponse.Message.Trim());
            Assert.AreEqual(voucherRequest.VoucherCode, voucherResponse.VoucherCode);
        }

        [Test]
        public async Task VoucherCheckServiceGetVoucherDecryptVoucherResponseNegativePathTests()
        {
            var voucherRequest = setuptestData("12345");

            _encryptionService.Setup(x => x.Decrypt(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());

            VoucherCheckResponse voucherResponse = await _voucherCheckService.GetVoucherResponse(voucherRequest);

            Assert.AreEqual("ERROR", voucherResponse.Status);
            Assert.AreEqual(10, voucherResponse.ErrorCode);
            //Assert.AreEqual("Unknown token", voucherResponse.message.Trim());
            Assert.AreEqual(voucherRequest.VoucherCode, voucherResponse.VoucherCode);
        }

        [Test]
        public async Task VoucherCheckServiceGetVoucherDecryptCanelledVoucherStatus1Response()
        {
            var voucherRequest = setuptestData("12345", 1);

            VoucherCheckResponse voucherResponse = await _voucherCheckService.GetVoucherResponse(voucherRequest);

            Assert.AreEqual("ERROR", voucherResponse.Status);
            Assert.AreEqual(50, voucherResponse.ErrorCode);
            //Assert.AreEqual("Unknown token", voucherResponse.message.Trim());
            Assert.AreEqual(voucherRequest.VoucherCode, voucherResponse.VoucherCode);
        }

        [Test]
        public async Task VoucherCheckServiceGetVoucherDecryptCanelledVoucherStatus2Response()
        {
            var voucherRequest = setuptestData("12345", 2);

            VoucherCheckResponse voucherResponse = await _voucherCheckService.GetVoucherResponse(voucherRequest);

            Assert.AreEqual("ERROR", voucherResponse.Status);
            Assert.AreEqual(50, voucherResponse.ErrorCode);
            //Assert.AreEqual("Unknown token", voucherResponse.message.Trim());
            Assert.AreEqual(voucherRequest.VoucherCode, voucherResponse.VoucherCode);
        }

        [Test]
        public async Task VoucherCheckServiceGetVoucherDecryptCanelledVoucherStatus3Response()
        {
            var voucherRequest = setuptestData("12345", 3);

            VoucherCheckResponse voucherResponse = await _voucherCheckService.GetVoucherResponse(voucherRequest);

            Assert.AreEqual("ERROR", voucherResponse.Status);
            Assert.AreEqual(50, voucherResponse.ErrorCode);
            //Assert.AreEqual("Unknown token", voucherResponse.message.Trim());
            Assert.AreEqual(voucherRequest.VoucherCode, voucherResponse.VoucherCode);
        }

        [Test]
        public async Task VoucherCheckServiceGetVoucherDecryptCanelledVoucherStatus4Response()
        {
            var voucherRequest = setuptestData("12345", 4);

            VoucherCheckResponse voucherResponse = await _voucherCheckService.GetVoucherResponse(voucherRequest);

            Assert.AreEqual("ERROR", voucherResponse.Status);
            Assert.AreEqual(50, voucherResponse.ErrorCode);            
            Assert.AreEqual(voucherRequest.VoucherCode, voucherResponse.VoucherCode);
        }

        private VoucherCheckRequest setuptestData(string registrationNumber, int? tokenCancellationCode = null)
        {
            VoucherCheckRequest voucherRequest = new VoucherCheckRequest()
            {
                Registration = registrationNumber,
                AccessCode = "12345",
                VoucherCode = "IvMBLZ2PhUVkmJHpAxle0Q"
            };

            var vendor_company = new vendor_company()
            {
                vendorid = 12345,
                registration_id = "12345",
                access_secret = "12345"
            };
            
            var token = new token()
            {
                token_id = 123456,
                token_code = "abcdef",
                product = 1234567,
                token_expiry = DateTime.Now.AddDays(30),
                token_balance = 5000,
                authorisation_code = "ABD",
                token_Cancellation_Status = tokenCancellationCode.HasValue ? new token_cancellation_status {  cancellation_status_id = tokenCancellationCode.Value} : null,
                cancellation_status_id = tokenCancellationCode
            };
            
            var product = new product()
            {
                product_id = 1234567,
                vendor_id = 12345,
                product_name = "abcdef"
            };
            
            var enterprise = new enterprise()
            {
                enterprise_id = 12345,
                enterprise_name = "abcdefgh"
            };
            
            _vendorCompanyRepository.Setup(x => x.GetVendorCompanyByRegistration(It.IsAny<string>()))
                .Returns(vendor_company);
            _encryptionService.Setup(x => x.Decrypt(It.IsAny<string>(), It.IsAny<string>())).Returns("123456");
            _tokenRepository.Setup(x => x.GetTokenByTokenCode(It.IsAny<string>())).Returns(token);
            _productRepository.Setup(x => x.GetProductSingle(It.IsAny<long>())).ReturnsAsync(product);
            _enterpriseRepository.Setup(x => x.GetEnterprise(It.IsAny<long>())).ReturnsAsync(enterprise);
            return voucherRequest;
        }
    }
}