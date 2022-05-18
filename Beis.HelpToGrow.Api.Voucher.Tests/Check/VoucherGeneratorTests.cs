using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Api.Voucher.Tests.Check
{
    [TestFixture]
    public class VoucherGeneratorTests
    {
        private GenerateVoucherController _voucherGenerator;
        private Mock<ITokenVoucherGeneratorService> _tokenVoucherGeneratorService;
        private Mock<ILogger<VoucherCheckController>> _logger;

        [SetUp]
        public void Setup()
        {

            _tokenVoucherGeneratorService = new Mock<ITokenVoucherGeneratorService>();
            _logger = new Mock<ILogger<VoucherCheckController>>();
            _voucherGenerator = new GenerateVoucherController(_logger.Object, _tokenVoucherGeneratorService.Object);
        }
        
        [Test]
        public async Task CheckVoucherReturnsErrorCodeZero()
        {
            var request = new VoucherGenerationRequest
            {
                productSku = "sku123",
                registration = "12345"
            };

            _tokenVoucherGeneratorService.Setup(x => x.GenerateVoucher(It.IsAny<VoucherGenerationRequest>())).ReturnsAsync(GetVoucherResponse(request));

            var actionResult = await _voucherGenerator.GenerateVoucher(request);

            var voucherResponse = actionResult.Value;

            Assert.AreEqual(0, voucherResponse.errorCode);
            Assert.AreEqual(request.productSku, voucherResponse.productSku);
            Assert.AreEqual(request.registration, voucherResponse.registration);
        }

        [Test]
        public async Task CheckVoucherReturnsErrorCode400()
        {
            var request = new VoucherGenerationRequest
            {
                productSku = "sku123",
                registration = "12345"
            };

            var expectedResponse = GetVoucherResponse(request);
            expectedResponse.errorCode = 400;
            expectedResponse.voucherCode = "voucherCode";
            expectedResponse.voucherBalance = 12.34M;

            _tokenVoucherGeneratorService.Setup(x => x.GenerateVoucher(It.IsAny<VoucherGenerationRequest>())).ReturnsAsync(expectedResponse);

            var actionResult = await _voucherGenerator.GenerateVoucher(request);

            var voucherResponse = (VoucherGenerationResponse)((ObjectResult)actionResult.Result).Value;

            Assert.AreEqual(400, voucherResponse.errorCode);
            Assert.AreEqual("voucherCode", voucherResponse.voucherCode);
            Assert.AreEqual(12.34M, voucherResponse.voucherBalance);
            Assert.AreEqual(request.productSku, voucherResponse.productSku);
            Assert.AreEqual(request.registration, voucherResponse.registration);
        }

        [Test]
        public async Task CheckVoucherReturnsException()
        {
            var request = new VoucherGenerationRequest
            {
                productSku = "sku123",
                registration = "12345",
            };

            var expectedResponse = GetVoucherResponse(request);
            expectedResponse.errorCode = 400;
            _tokenVoucherGeneratorService.Setup(x => x.GenerateVoucher(It.IsAny<VoucherGenerationRequest>())).Throws(new Exception());

            var actionResult = await _voucherGenerator.GenerateVoucher(request);

            var voucherResponse = (VoucherGenerationResponse)((ObjectResult)actionResult.Result).Value;

            Assert.AreEqual(500, ((ObjectResult)actionResult.Result).StatusCode);
            Assert.AreEqual(10, voucherResponse.errorCode);
            Assert.AreEqual("ERROR", voucherResponse.status);
            Assert.AreEqual(request.productSku, voucherResponse.productSku);
            Assert.AreEqual(request.registration, voucherResponse.registration);
            Assert.AreEqual("Unknown ProductSKU or Vendor details", voucherResponse.message);
        }

        private VoucherGenerationResponse GetVoucherResponse(VoucherGenerationRequest voucherRequest)
        {
            return new VoucherGenerationResponse()
            {
                errorCode = 0,
                productSku = voucherRequest.productSku,
                registration = voucherRequest.registration
            }; 
        }        
    }
}