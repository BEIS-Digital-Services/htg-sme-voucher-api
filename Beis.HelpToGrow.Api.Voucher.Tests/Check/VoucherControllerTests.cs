using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Beis.HelpToGrow.Api.Voucher.Tests.Check
{
    [TestFixture]
    public class VoucherControllerTests
    {
        private VoucherCheckController _voucherController;
        private Mock<IVoucherCheckService> _voucherCheckService;
        private Mock<ILogger<VoucherCheckController>> _logger;
        private Mock<IVendorAPICallStatusServices> _vendorApiCallStatusServices;

        [SetUp]
        public void Setup()
        {
            
            _voucherCheckService = new Mock<IVoucherCheckService>();
            _logger = new Mock<ILogger<VoucherCheckController>>();
            _vendorApiCallStatusServices = new Mock<IVendorAPICallStatusServices>();
            _vendorApiCallStatusServices.Setup(x => x.CreateLogRequestDetails(It.IsAny<ILogVoucherRequest>()))
                .Returns(new vendor_api_call_status { });
            _voucherController = new VoucherCheckController(_logger.Object, _voucherCheckService.Object, _vendorApiCallStatusServices.Object);
        }
        
        [Test]
        public async Task CheckVoucherHappyPathTest ()
        {
            VoucherCheckRequest voucherRequest = new VoucherCheckRequest();
            
            voucherRequest.Registration = "12345";
            voucherRequest.AccessCode = "12345";
            voucherRequest.VoucherCode = "IvMBLZ2PhUVkmJHpAxle0Q";
            
            _voucherCheckService.Setup(x => x.GetVoucherResponse(It.IsAny<VoucherCheckRequest>())).ReturnsAsync(getVoucherResponse(voucherRequest));
            
            ActionResult<VoucherCheckResponse> actionResult = await _voucherController.CheckVoucher(voucherRequest);

            VoucherCheckResponse voucherResponse = (VoucherCheckResponse) ((OkObjectResult) actionResult.Result).Value;
            
            Assert.AreEqual(voucherRequest.VoucherCode, voucherResponse.VoucherCode);
            Assert.AreEqual(0, voucherResponse.ErrorCode);
            Assert.AreEqual("GHT23RTDWER", voucherResponse.AuthorisationCode);
            Assert.AreEqual("ABC corporation", voucherResponse.Vendor);
            Assert.AreEqual("GHU1234", voucherResponse.ProductSku);
            Assert.AreEqual("Mr. Joe Blogs", voucherResponse.PurchaserName);
            Assert.AreEqual("Buyer limited", voucherResponse.LicenceTo);
            Assert.AreEqual("abc@my-company.com", voucherResponse.SmeEmail);
            Assert.AreEqual(4999, voucherResponse.MaxDiscountAllowed);
        }
       
        [Test]
        public async Task CheckVoucherNegativePathTest () 
        {
            VoucherCheckRequest voucherRequest = new VoucherCheckRequest();
            
            voucherRequest.Registration = "12345";
            voucherRequest.AccessCode = "12345";
            voucherRequest.VoucherCode = "IvMBLZ2PhUVkmJHpAxle0Qcc";
            
            _voucherCheckService.Setup(x => x.GetVoucherResponse(It.IsAny<VoucherCheckRequest>())).Throws(new Exception("my exception"));
            
            ActionResult<VoucherCheckResponse> actionResult = await _voucherController.CheckVoucher(voucherRequest);

            VoucherCheckResponse voucherResponse = (VoucherCheckResponse) ((ObjectResult) actionResult.Result).Value;
            
            Assert.AreEqual(voucherRequest.VoucherCode, voucherResponse.VoucherCode);
            Assert.AreEqual("ERROR", voucherResponse.Status);
            Assert.AreEqual(10, voucherResponse.ErrorCode);
            Assert.AreEqual("Unknown token my exception", voucherResponse.Message);
        }

        [Test]
        public async Task CheckVoucherReturnsStatus400PathTest()
        {
            VoucherCheckRequest voucherRequest = new VoucherCheckRequest();

            voucherRequest.Registration = "12345";
            voucherRequest.AccessCode = "12345";
            voucherRequest.VoucherCode = "IvMBLZ2PhUVkmJHpAxle0Qcc";

            var expectedResponse = new VoucherCheckResponse
            { 
                ErrorCode = 400,
                VoucherCode = "IvMBLZ2PhUVkmJHpAxle0Qcc",
                Status = "ERROR"
            };

            _voucherCheckService.Setup(x => x.GetVoucherResponse(It.IsAny<VoucherCheckRequest>())).ReturnsAsync(expectedResponse);

            ActionResult<VoucherCheckResponse> actionResult = await _voucherController.CheckVoucher(voucherRequest);

            VoucherCheckResponse voucherResponse = (VoucherCheckResponse)((ObjectResult)actionResult.Result).Value;

            Assert.AreEqual(voucherRequest.VoucherCode, voucherResponse.VoucherCode);
            Assert.AreEqual("ERROR", voucherResponse.Status);
            Assert.AreEqual(400, voucherResponse.ErrorCode);
        }

        private VoucherCheckResponse getVoucherResponse(VoucherCheckRequest voucherRequest)
        {
            return new VoucherCheckResponse()
            {
                VoucherCode = voucherRequest.VoucherCode,
                ErrorCode = 0,
                AuthorisationCode = "GHT23RTDWER",
                Vendor = "ABC corporation",
                ProductSku = "GHU1234",
                PurchaserName = "Mr. Joe Blogs",
                LicenceTo = "Buyer limited",
                SmeEmail = "abc@my-company.com",
                MaxDiscountAllowed = 4999
            }; }
        
    }
}