using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;


namespace Beis.HelpToGrow.Api.Voucher.Tests.Reconciliation
{
    [TestFixture]
    public class VoucherControllerTests
    {
        private VoucherReconciliationController _voucherController;
        private Mock<IVoucherReconciliationService> _voucherReconciliationService;
        private Mock<ILogger<VoucherReconciliationController>> _logger;
        private Mock<IVendorAPICallStatusServices> _vendorApiCallStatusServices;

        [SetUp] 
        public void Setup()
        {
            _voucherReconciliationService = new Mock<IVoucherReconciliationService>();
            _vendorApiCallStatusServices = new Mock<IVendorAPICallStatusServices>();
            _logger = new Mock<ILogger<VoucherReconciliationController>>();
            _voucherController = new VoucherReconciliationController(_logger.Object, _voucherReconciliationService.Object, _vendorApiCallStatusServices.Object);
        }

        [Test]
        public async Task VoucherReconciliationHappyTest()
        {
            VoucherReconciliationRequest voucherReconciliationRequest = new VoucherReconciliationRequest();
            voucherReconciliationRequest.Registration = "12345";
            voucherReconciliationRequest.AccessCode = "12345";
            voucherReconciliationRequest.ReconciliationDate = new DateTime();
            
            List<SalesReconcilliation> salesList = new List<SalesReconcilliation>();
            SalesReconcilliation salesReconcilliation = new SalesReconcilliation();

            salesReconcilliation.NotificationType = "new";
            salesReconcilliation.VoucherCode = "IvMBLZ2PhUVkmJHpAxle0Q";
            salesReconcilliation.AuthorisationCode = "GHT23RTDWER";
            salesReconcilliation.ProductSku = "GHU12234";
            salesReconcilliation.ProductName = "My Accounts Package";
            salesReconcilliation.LicenceTo = "Buyer limited";
            salesReconcilliation.SmeEmail = "abc@my-company.com";
            salesReconcilliation.PurchaserName = "Mr. Joe Blogs";
            salesReconcilliation.OneOffCosts =  30;
            salesReconcilliation.NoOfLicences = 30;
            salesReconcilliation.CostPerLicence =  15;
            salesReconcilliation.TotalAmount = 4999;
            salesReconcilliation.DiscountApplied = 2500;
            salesReconcilliation.Currency = "GBP";
            salesReconcilliation.ContractTermInMonths = 12;
            salesReconcilliation.TrialPeriodInMonths = 3;

            salesList.Add(salesReconcilliation);
            DailySales dailySalesTest = new DailySales();
            dailySalesTest.Sales = salesList;
            voucherReconciliationRequest.DailySales = dailySalesTest;

            var voucherResponse = new VoucherReconciliationResponse() { Status = "200",ErrorCode = 0 };
            _voucherReconciliationService.Setup(x => x.GetVoucherResponse(It.IsAny<VoucherReconciliationRequest>()))
                .ReturnsAsync(voucherResponse);

            var logRequestDetailsResponse = new vendor_api_call_status { error_code = "200" };
            _vendorApiCallStatusServices.Setup(x => x.CreateLogRequestDetails(It.IsAny<ILogVoucherRequest>()))
                .Returns(logRequestDetailsResponse);

            ActionResult<VoucherReconciliationResponse> actionResult = await _voucherController.CheckVoucher(voucherReconciliationRequest);

            VoucherReconciliationResponse actualVoucherResponse = (VoucherReconciliationResponse) ((OkObjectResult) actionResult.Result).Value;
            
            Assert.AreEqual("OK", voucherResponse.Status);
            Assert.AreEqual(0, voucherResponse.ErrorCode);

        }

        [Test]
        public async Task VoucherReconciliationNegativeTest()
        {
            VoucherReconciliationRequest voucherReconciliationRequest = new VoucherReconciliationRequest();
            voucherReconciliationRequest.Registration = "12345";
            voucherReconciliationRequest.AccessCode = "12345";
            voucherReconciliationRequest.ReconciliationDate = new DateTime();

            
            List<SalesReconcilliation> salesList = new List<SalesReconcilliation>();
            SalesReconcilliation salesReconcilliation = new SalesReconcilliation();

            salesReconcilliation.NotificationType = "new";
            salesReconcilliation.VoucherCode = "IvMBLZ2PhUVkmJHpAxle0Q558";
            salesReconcilliation.AuthorisationCode = "GHT23RTDWER";
            salesReconcilliation.ProductSku = "GHU12234";
            salesReconcilliation.ProductName = "My Accounts Package";
            salesReconcilliation.LicenceTo = "Buyer limited";
            salesReconcilliation.SmeEmail = "abc@my-company.com";
            salesReconcilliation.PurchaserName = "Mr. Joe Blogs";
            salesReconcilliation.OneOffCosts = 30;
            salesReconcilliation.NoOfLicences = 30;
            salesReconcilliation.CostPerLicence = 15;
            salesReconcilliation.TotalAmount = 4999;
            salesReconcilliation.DiscountApplied = 2500;
            salesReconcilliation.Currency = "GBP";
            salesReconcilliation.ContractTermInMonths = 12;
            salesReconcilliation.TrialPeriodInMonths = 3;

            salesList.Add(salesReconcilliation);
            DailySales dailySalesTest = new DailySales();
            dailySalesTest.Sales = salesList;
            voucherReconciliationRequest.DailySales = dailySalesTest;

            var logRequestDetailsResponse = new vendor_api_call_status { error_code = "200" };
            _vendorApiCallStatusServices.Setup(x => x.CreateLogRequestDetails(It.IsAny<ILogVoucherRequest>()))
                .Returns(logRequestDetailsResponse);

            _voucherReconciliationService.Setup(x => x.GetVoucherResponse(It.IsAny<VoucherReconciliationRequest>()))
                .Throws(new Exception("Error here"));


            ActionResult<VoucherReconciliationResponse> actionResult = await _voucherController.CheckVoucher(voucherReconciliationRequest);

            VoucherReconciliationResponse voucherResponse =
                (VoucherReconciliationResponse) ((ObjectResult) actionResult.Result).Value;
            
            Assert.AreEqual("ERROR", voucherResponse.Status);
            Assert.AreEqual(0, voucherResponse.ErrorCode);
            Assert.AreEqual("An error has occurred - Check Error code and Message", voucherResponse.Message);
        }
    }
}
