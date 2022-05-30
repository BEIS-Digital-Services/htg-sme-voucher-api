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
            _vendorApiCallStatusServices.Setup(x => x.CreateLogRequestDetails(It.IsAny<VoucherCheckRequest>()))
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
            
            Assert.AreEqual(voucherRequest.VoucherCode, voucherResponse.voucherCode);
            Assert.AreEqual(0, voucherResponse.errorCode);
            Assert.AreEqual("GHT23RTDWER", voucherResponse.authorisationCode);
            Assert.AreEqual("ABC corporation", voucherResponse.vendor);
            Assert.AreEqual("GHU1234", voucherResponse.productSku);
            Assert.AreEqual("Mr. Joe Blogs", voucherResponse.purchaserName);
            Assert.AreEqual("Buyer limited", voucherResponse.licenceTo);
            Assert.AreEqual("abc@my-company.com", voucherResponse.smeEmail);
            Assert.AreEqual(4999, voucherResponse.maxDiscountAllowed);
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
            
            Assert.AreEqual(voucherRequest.VoucherCode, voucherResponse.voucherCode);
            Assert.AreEqual("ERROR", voucherResponse.status);
            Assert.AreEqual(10, voucherResponse.errorCode);
            Assert.AreEqual("Unknown token my exception", voucherResponse.message);
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
                errorCode = 400,
                voucherCode = "IvMBLZ2PhUVkmJHpAxle0Qcc",
                status = "ERROR"
            };

            _voucherCheckService.Setup(x => x.GetVoucherResponse(It.IsAny<VoucherCheckRequest>())).ReturnsAsync(expectedResponse);

            ActionResult<VoucherCheckResponse> actionResult = await _voucherController.CheckVoucher(voucherRequest);

            VoucherCheckResponse voucherResponse = (VoucherCheckResponse)((ObjectResult)actionResult.Result).Value;

            Assert.AreEqual(voucherRequest.VoucherCode, voucherResponse.voucherCode);
            Assert.AreEqual("ERROR", voucherResponse.status);
            Assert.AreEqual(400, voucherResponse.errorCode);
        }

        private VoucherCheckResponse getVoucherResponse(VoucherCheckRequest voucherRequest)
        {
            return new VoucherCheckResponse()
            {
                voucherCode = voucherRequest.VoucherCode,
                errorCode = 0,
                authorisationCode = "GHT23RTDWER",
                vendor = "ABC corporation",
                productSku = "GHU1234",
                purchaserName = "Mr. Joe Blogs",
                licenceTo = "Buyer limited",
                smeEmail = "abc@my-company.com",
                maxDiscountAllowed = 4999
            }; }
        
    }
}