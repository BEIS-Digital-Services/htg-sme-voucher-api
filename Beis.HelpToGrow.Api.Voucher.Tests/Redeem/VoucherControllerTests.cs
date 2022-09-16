using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Serialization;
using NuGet.Frameworks;
using NUnit.Framework;
using System.Threading.Tasks;


namespace Beis.HelpToGrow.Api.Voucher.Tests.Redeem
{
    [TestFixture]
    public class VoucherControllerTests
    {
        private VoucherRedeemController _voucherController;
        private Mock<IVoucherRedeemService> _voucherRedeemService;
        private Mock<IVendorAPICallStatusServices> _vendorAPICallStatusServices;
        private Mock<ILogger<VoucherRedeemController>> _logger;

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<VoucherRedeemController>>();
            _vendorAPICallStatusServices = new Mock<IVendorAPICallStatusServices>();
            _vendorAPICallStatusServices.Setup(x => x.CreateLogRequestDetails(It.IsAny<ILogVoucherRequest>()))
                .Returns(new vendor_api_call_status { });
           _voucherRedeemService = new Mock<IVoucherRedeemService>();
            _voucherController = new VoucherRedeemController(_logger.Object, _voucherRedeemService.Object, _vendorAPICallStatusServices.Object);
        }

        [Test]
        public async Task RedeemVoucherHappyPathTest()
        {
            VoucherUpdateRequest voucherUpdateRequest = new VoucherUpdateRequest()
            {
                Registration = "12345",
                AccessCode = "12345",
                VoucherCode = "IvMBLZ2PhUVkmJHpAxle0Q",
                AuthorisationCode = "GHT23RTDWER"
            };

            var voucherUpdateExpectedResponse = new VoucherUpdateResponse()
            {
                VoucherCode = "IvMBLZ2PhUVkmJHpAxle0Q",
                Status = "OK",
                ErrorCode = 0,
                Message = "XXXXXXXXX"
            };
            
            _voucherRedeemService.Setup(x =>x.GetVoucherResponse(It.IsAny<VoucherUpdateRequest>())).ReturnsAsync(voucherUpdateExpectedResponse);
            ActionResult<VoucherUpdateResponse> actionResult = await _voucherController.CheckVoucher(voucherUpdateRequest);
            VoucherUpdateResponse voucherUpdateResponse = (VoucherUpdateResponse) ((OkObjectResult) actionResult.Result).Value;

            Assert.AreEqual(voucherUpdateRequest.VoucherCode, voucherUpdateResponse.VoucherCode);
            Assert.AreEqual("OK", voucherUpdateResponse.Status);
            Assert.AreEqual(0, voucherUpdateResponse.ErrorCode);
            Assert.AreEqual("Successful check - proceed", voucherUpdateResponse.Message);
        }

        [Test]
        public async Task RedeemVoucherNegativePathTest()
        {
            VoucherUpdateRequest voucherUpdateRequest = new VoucherUpdateRequest()
            {
                Registration = "12345",
                AccessCode = "12345",
                VoucherCode = "IvMBLZ2PhUVkmJHpAxle0Q99",
                AuthorisationCode = "GHT23RTDWER"
            };

            var voucherUpdateExpectedResponse = new VoucherUpdateResponse()
            {
                VoucherCode = "IvMBLZ2PhUVkmJHpAxle0Q",
                Status = "ERROR",
                ErrorCode = 401,
                Message = "Invalid voucher code"
            };
            
            _voucherRedeemService.Setup(x =>x.GetVoucherResponse(It.IsAny<VoucherUpdateRequest>())).ReturnsAsync(voucherUpdateExpectedResponse);
            ActionResult<VoucherUpdateResponse> actionResult = await _voucherController.CheckVoucher(voucherUpdateRequest);
            VoucherUpdateResponse voucherUpdateResponse = (VoucherUpdateResponse) ((ObjectResult) actionResult.Result).Value;
            
            Assert.AreEqual("ERROR", voucherUpdateResponse.Status);
            Assert.AreEqual(401, voucherUpdateResponse.ErrorCode);
            Assert.AreEqual("Invalid voucher code", voucherUpdateResponse.Message);
        }
        

    }
}