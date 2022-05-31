using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Api.Voucher.Tests.Check
{
    [TestFixture]
    public class TokenVoucherGeneratorServiceTests
    {
        private TokenVoucherGeneratorService _service;
        private Mock<IVoucherGenerationService> _voucherGeneratorService;
        private Mock<IEncryptionService> _encryptionService;
        private Mock<ITokenRepository> _tokenRepository;
        private Mock<IProductRepository> _productRepository;
        private Mock<IVendorCompanyRepository> _vendorCompanyRepository;

        private Mock<IEnterpriseRepository> _enterpriseRepository;
        private Mock<ILogger<TokenVoucherGeneratorService>> _logger;
        private IOptions<VoucherSettings> voucherOptions;

        [SetUp]

        public void Setup()
        {
            _encryptionService = new Mock<IEncryptionService>();
            _voucherGeneratorService = new Mock<IVoucherGenerationService>();
            _tokenRepository = new Mock<ITokenRepository>();
            _productRepository = new Mock<IProductRepository>();
            _vendorCompanyRepository = new Mock<IVendorCompanyRepository>();
            _enterpriseRepository = new Mock<IEnterpriseRepository>();
            _logger = new Mock<ILogger<TokenVoucherGeneratorService>>();
            voucherOptions = Options.Create(new VoucherSettings { VoucherCodeLength = 9 });
            _service = new TokenVoucherGeneratorService(_productRepository.Object, _vendorCompanyRepository.Object, _voucherGeneratorService.Object, voucherOptions );
        }

        [Test]

        public async Task CallingGenerateVoucherHappyPathTests()
        {
            var request = SetupTestData("12345");

            var voucherResponse = await _service.GenerateVoucher(request);
            
            Assert.NotNull(voucherResponse);
            Assert.AreEqual(typeof(VoucherGenerationResponse), voucherResponse.GetType());
            Assert.AreEqual(request.Registration, voucherResponse.Registration);
            Assert.AreEqual("voucherCode", voucherResponse.VoucherCode);
            Assert.AreEqual(5000M, voucherResponse.VoucherBalance);
        }

        [Test]

        public async Task CallingGenerateVoucherProductNotFoundTests()
        {
            var request = SetupTestData("12345");

            _productRepository.Setup(x => x.GetProductBySku(It.IsAny<string>(), It.IsAny<long>())).Returns((product)null);
            var voucherResponse = await _service.GenerateVoucher(request);

            Assert.NotNull(voucherResponse);
            Assert.AreEqual(typeof(VoucherGenerationResponse), voucherResponse.GetType());
            Assert.AreEqual("ERROR", voucherResponse.Status);
            Assert.AreEqual("Product not found.", voucherResponse.Message);
        }

        [Test]

        public async Task CallingGenerateVoucherCompanyRegistrationNotFoundTests()
        {
            var request = SetupTestData("12345");

            _vendorCompanyRepository.Setup(x => x.GetVendorCompanyByRegistration(It.IsAny<string>())).Returns((vendor_company)null);
            var voucherResponse = await _service.GenerateVoucher(request);

            Assert.NotNull(voucherResponse);
            Assert.AreEqual(typeof(VoucherGenerationResponse), voucherResponse.GetType());
            Assert.AreEqual("ERROR", voucherResponse.Status);
            Assert.AreEqual("Company registration not found.", voucherResponse.Message);
        }

        private VoucherGenerationRequest SetupTestData(string registrationNumber)
        {
            var voucherRequest = new VoucherGenerationRequest()
            {
                Registration = registrationNumber,
                ProductSku = "sku123",
            };

            var vendor_company = new vendor_company()
            {
                vendorid = 12345,
                registration_id = "12345"
            };
            
            var product = new product()
            {
                product_id = 1234567
            };
            
            var Enterprise = new enterprise()
            {
                enterprise_id = 12345
            };

            var token = new token()
            {
                token_id = 123456,
                token_code = "abcdef",
                product = 1234567,
                token_expiry = DateTime.Now.AddDays(30),
                token_balance = 12.34M,
                authorisation_code = "ABD"
            };

            _vendorCompanyRepository.Setup(x => x.GetVendorCompanyByRegistration(It.IsAny<string>())).Returns(vendor_company);
            _productRepository.Setup(x => x.GetProductBySku(It.IsAny<string>(), It.IsAny<long>())).Returns(product);
            _voucherGeneratorService.Setup(x => x.GenerateVoucher(It.IsAny<vendor_company>(), It.IsAny<enterprise>(), It.IsAny<product>(), It.IsAny<IOptions<VoucherSettings>>(),It.IsAny<bool>())).ReturnsAsync("voucherCode");

            return voucherRequest;
        }
    }
}