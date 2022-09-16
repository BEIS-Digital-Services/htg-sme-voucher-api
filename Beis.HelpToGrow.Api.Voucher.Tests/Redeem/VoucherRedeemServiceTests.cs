using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;


namespace Beis.HelpToGrow.Api.Voucher.Tests.Redeem
{
    public class VoucherRedeemServiceTests
    {
        private VoucherRedeemService _voucherRedeemService;
        private Mock<IEncryptionService> _encryptionService;
        private Mock<ITokenRepository> _tokenRepository;
        private Mock<IProductRepository> _productRepository;
        private Mock<IVendorCompanyRepository> _vendorCompanyRepository;
        private Mock<ILogger<VoucherRedeemService>> _logger;
        private Mock<IVendorAPICallStatusServices> _vendorAPICallStatusServices;

        [SetUp]
        public void Setup()
        {
            _encryptionService = new Mock<IEncryptionService>();
            _tokenRepository = new Mock<ITokenRepository>();
            _productRepository = new Mock<IProductRepository>();
            _vendorCompanyRepository = new Mock<IVendorCompanyRepository>();
            _logger = new Mock<ILogger<VoucherRedeemService>>();
            _vendorAPICallStatusServices = new Mock<IVendorAPICallStatusServices>();
            _vendorAPICallStatusServices.Setup(x => x.CreateLogRequestDetails(It.IsAny<ILogVoucherRequest>()))
                .Returns(new vendor_api_call_status { });

            _voucherRedeemService = new VoucherRedeemService(_logger.Object,
                                                                                                _encryptionService.Object,
                                                                                                _tokenRepository.Object,
                                                                                                _vendorCompanyRepository.Object,
                                                                                                _vendorAPICallStatusServices.Object);
        }

        [Test]
        public async Task VoucherRedeemServiceGetVoucherResponseHappyTests()
        {
            var fakeToken = FakeToken;
            fakeToken.token_expiry = DateTime.Now.AddMinutes(5);

            var voucherUpdateRequest = SetupVoucherUpdateRequest("12345", FakeVendorCompany, fakeToken);

            var voucherUpdateResponse = await _voucherRedeemService.GetVoucherResponse(voucherUpdateRequest);

            Assert.AreEqual(0, voucherUpdateResponse.ErrorCode);
            Assert.AreEqual("OK", voucherUpdateResponse.Status);
            Assert.AreEqual("abcdef", voucherUpdateResponse.VoucherCode);
        }

        [Test]
        public async Task VoucherRedeemServiceGetVoucherResponseNegativeTests()
        {
            var voucherUpdateRequest = SetupVoucherUpdateRequest("99999");

            VoucherUpdateResponse voucherUpdateResponse = await _voucherRedeemService.GetVoucherResponse(voucherUpdateRequest);

            Assert.AreEqual(10, voucherUpdateResponse.ErrorCode);
            Assert.AreEqual("Unknown token or company", voucherUpdateResponse.Message.Trim());
            Assert.AreEqual("ERROR", voucherUpdateResponse.Status);
            Assert.AreEqual("abcdef", voucherUpdateResponse.VoucherCode);
        }

        [Test]
        public async Task VoucherRedeemServiceGetVoucherResponseCancellationStatus1Response()
        {
            var voucherUpdateRequest = SetupVoucherUpdateRequest("99999", tokenCancellationCode: 1);

            var voucherUpdateResponse = await _voucherRedeemService.GetVoucherResponse(voucherUpdateRequest);

            Assert.AreEqual(40, voucherUpdateResponse.ErrorCode);
            Assert.AreEqual("Cancelled token", voucherUpdateResponse.Message.Trim());
            Assert.AreEqual("ERROR", voucherUpdateResponse.Status);
            Assert.AreEqual("abcdef", voucherUpdateResponse.VoucherCode);
        }

        [Test]
        public async Task VoucherRedeemServiceGetVoucherResponseCancellationStatus2Response()
        {
            var voucherUpdateRequest = SetupVoucherUpdateRequest("99999", tokenCancellationCode: 2);

            var voucherUpdateResponse = await _voucherRedeemService.GetVoucherResponse(voucherUpdateRequest);

            Assert.AreEqual(40, voucherUpdateResponse.ErrorCode);
            Assert.AreEqual("Cancelled token", voucherUpdateResponse.Message.Trim());
            Assert.AreEqual("ERROR", voucherUpdateResponse.Status);
            Assert.AreEqual("abcdef", voucherUpdateResponse.VoucherCode);
        }

        [Test]
        public async Task VoucherRedeemServiceGetVoucherResponseCancellationStatus3Response()
        {
            var voucherUpdateRequest = SetupVoucherUpdateRequest("99999", tokenCancellationCode: 3);

            var voucherUpdateResponse = await _voucherRedeemService.GetVoucherResponse(voucherUpdateRequest);

            Assert.AreEqual(40, voucherUpdateResponse.ErrorCode);
            Assert.AreEqual("Cancelled token", voucherUpdateResponse.Message.Trim());
            Assert.AreEqual("ERROR", voucherUpdateResponse.Status);
            Assert.AreEqual("abcdef", voucherUpdateResponse.VoucherCode);
        }

        [Test]
        public async Task VoucherRedeemServiceGetVoucherResponseCancellationStatus4Response()
        {
            var voucherUpdateRequest = SetupVoucherUpdateRequest("99999", tokenCancellationCode: 4);

            var voucherUpdateResponse = await _voucherRedeemService.GetVoucherResponse(voucherUpdateRequest);

            Assert.AreEqual(40, voucherUpdateResponse.ErrorCode);
            Assert.AreEqual("Cancelled token", voucherUpdateResponse.Message.Trim());
            Assert.AreEqual("ERROR", voucherUpdateResponse.Status);
            Assert.AreEqual("abcdef", voucherUpdateResponse.VoucherCode);
        }

        private VoucherUpdateRequest SetupVoucherUpdateRequest(string registrationNumber, vendor_company vendorCompanyArg = null, token tokenArg = null, product productArg = null, int? tokenCancellationCode = null)
        {
            var voucherUpdateRequest = new VoucherUpdateRequest
            {
                Registration = registrationNumber,
                AccessCode = "12345",
                VoucherCode = "abcdef",
                AuthorisationCode = "abcd"
            };

            var vendorCompany = vendorCompanyArg ?? FakeVendorCompany;

            var token = tokenArg is { authorisation_code: "TOKEN UNKNOWN" }
                ? null
                : tokenArg ?? FakeToken;

            if (token != null && tokenCancellationCode.HasValue)
            {
                token.cancellation_status_id = tokenCancellationCode;
                token.token_Cancellation_Status = new token_cancellation_status { cancellation_status_id = tokenCancellationCode.Value };
            }

            var product = productArg ?? new product
            {
                product_id = 1234567,
                vendor_id = 12345,
                product_name = "abcdef"
            };

            _vendorCompanyRepository
                .Setup(x => x.GetVendorCompanyByRegistration(It.IsAny<string>()))
                .Returns(vendorCompany);

            _encryptionService
                .Setup(x => x.Decrypt(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("123456");

            _tokenRepository
                .Setup(x => x.GetTokenByTokenCode(It.IsAny<string>()))
                .Returns(token);

            _productRepository
                .Setup(x => x.GetProductSingle(It.IsAny<long>()))
                .Returns(Task.FromResult(product));

            return voucherUpdateRequest;
        }

        private static token FakeToken =>
            new()
            {
                token_id = 123456,
                token_code = "abcdef",
                product = 1234567,
                authorisation_code = "abcd"
            };

        private static vendor_company FakeVendorCompany =>
            new()
            {
                vendorid = 12345,
                registration_id = "12345",
                access_secret = "12345"
            };
    }
}