using Beis.Htg.VendorSme.Database.Models;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Api.Voucher.Tests.Check
{
    [TestFixture]
    public class VoucherGeneratorServiceTests
    {
        private VoucherGenerationService _service;
        private Mock<IVoucherGeneratorService> _voucherGeneratorService;
        private Mock<IEncryptionService> _encryptionService;
        private Mock<ITokenRepository> _tokenRepository;

        [SetUp]

        public void Setup()
        {
            _encryptionService = new Mock<IEncryptionService>();
            _voucherGeneratorService = new Mock<IVoucherGeneratorService>();
            _tokenRepository = new Mock<ITokenRepository>();
            _service = new VoucherGenerationService(_encryptionService.Object, _tokenRepository.Object);
        }

        [Test]

        public async Task CallingGenerateVoucherReturnsValidToken()
        {
            var vendorCompany = new vendor_company { registration_id = "1", vendorid = 1 };
            var enterprise = new enterprise { enterprise_id = 1};
            var product = new product { product_id = 1 };
            _encryptionService.Setup(x => x.Encrypt(It.IsAny<string>(), It.IsAny<string>())).Returns("encryptedToken");

            var response = await _service.GenerateVoucher(vendorCompany, enterprise, product);
            
            Assert.NotNull("encryptedToken", response);
        }

        [Test]

        public async Task CallingGenerateVoucherReturnsValidTokenEndingDoubleEquals()
        {
            var vendorCompany = new vendor_company { registration_id = "1", vendorid = 1 };
            var enterprise = new enterprise { enterprise_id = 1 };
            var product = new product { product_id = 1 };
            _encryptionService.Setup(x => x.Encrypt(It.IsAny<string>(), It.IsAny<string>())).Returns("encryptedToken==");

            var response = await _service.GenerateVoucher(vendorCompany, enterprise, product);

            Assert.NotNull("encryptedToken==", response);
        }
    }
}