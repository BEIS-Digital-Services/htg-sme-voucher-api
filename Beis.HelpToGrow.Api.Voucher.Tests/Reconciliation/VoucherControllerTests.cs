using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beis.Htg.VendorSme.Database.Models;
using Microsoft.AspNetCore.Mvc;
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

            salesReconcilliation.notificationType = "new";
            salesReconcilliation.voucherCode = "IvMBLZ2PhUVkmJHpAxle0Q";
            salesReconcilliation.authorisationCode = "GHT23RTDWER";
            salesReconcilliation.productSku = "GHU12234";
            salesReconcilliation.productName = "My Accounts Package";
            salesReconcilliation.licenceTo = "Buyer limited";
            salesReconcilliation.smeEmail = "abc@my-company.com";
            salesReconcilliation.purchaserName = "Mr. Joe Blogs";
            salesReconcilliation.oneOffCosts =  30;
            salesReconcilliation.noOfLicences = 30;
            salesReconcilliation.costPerLicence =  15;
            salesReconcilliation.totalAmount = 4999;
            salesReconcilliation.discountApplied = 2500;
            salesReconcilliation.currency = "GBP";
            salesReconcilliation.contractTermInMonths = 12;
            salesReconcilliation.trialPeriodInMonths = 3;

            salesList.Add(salesReconcilliation);
            DailySales dailySalesTest = new DailySales();
            dailySalesTest.sales = salesList;
            voucherReconciliationRequest.DailySales = dailySalesTest;

            var voucherResponse = new VoucherReconciliationResponse() { status = "200",errorCode = 0 };
            _voucherReconciliationService.Setup(x => x.GetVoucherResponse(It.IsAny<VoucherReconciliationRequest>()))
                .ReturnsAsync(voucherResponse);

            var logRequestDetailsResponse = new vendor_api_call_status { error_code = "200" };
            _vendorApiCallStatusServices.Setup(x => x.CreateLogRequestDetails(It.IsAny<VoucherReconciliationRequest>()))
                .Returns(logRequestDetailsResponse);

            ActionResult<VoucherReconciliationResponse> actionResult = await _voucherController.CheckVoucher(voucherReconciliationRequest);

            VoucherReconciliationResponse actualVoucherResponse = (VoucherReconciliationResponse) ((OkObjectResult) actionResult.Result).Value;
            
            Assert.AreEqual("OK", voucherResponse.status);
            Assert.AreEqual(0, voucherResponse.errorCode);

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

            salesReconcilliation.notificationType = "new";
            salesReconcilliation.voucherCode = "IvMBLZ2PhUVkmJHpAxle0Q558";
            salesReconcilliation.authorisationCode = "GHT23RTDWER";
            salesReconcilliation.productSku = "GHU12234";
            salesReconcilliation.productName = "My Accounts Package";
            salesReconcilliation.licenceTo = "Buyer limited";
            salesReconcilliation.smeEmail = "abc@my-company.com";
            salesReconcilliation.purchaserName = "Mr. Joe Blogs";
            salesReconcilliation.oneOffCosts = 30;
            salesReconcilliation.noOfLicences = 30;
            salesReconcilliation.costPerLicence = 15;
            salesReconcilliation.totalAmount = 4999;
            salesReconcilliation.discountApplied = 2500;
            salesReconcilliation.currency = "GBP";
            salesReconcilliation.contractTermInMonths = 12;
            salesReconcilliation.trialPeriodInMonths = 3;

            salesList.Add(salesReconcilliation);
            DailySales dailySalesTest = new DailySales();
            dailySalesTest.sales = salesList;
            voucherReconciliationRequest.DailySales = dailySalesTest;

            var logRequestDetailsResponse = new vendor_api_call_status { error_code = "200" };
            _vendorApiCallStatusServices.Setup(x => x.CreateLogRequestDetails(It.IsAny<VoucherReconciliationRequest>()))
                .Returns(logRequestDetailsResponse);

            _voucherReconciliationService.Setup(x => x.GetVoucherResponse(It.IsAny<VoucherReconciliationRequest>()))
                .Throws(new Exception("Error here"));


            ActionResult<VoucherReconciliationResponse> actionResult = await _voucherController.CheckVoucher(voucherReconciliationRequest);

            VoucherReconciliationResponse voucherResponse =
                (VoucherReconciliationResponse) ((ObjectResult) actionResult.Result).Value;
            
            Assert.AreEqual("ERROR", voucherResponse.status);
            Assert.AreEqual(0, voucherResponse.errorCode);
            Assert.AreEqual("An error has occurred - Check Error code and Message", voucherResponse.message);
        }
    }
}
