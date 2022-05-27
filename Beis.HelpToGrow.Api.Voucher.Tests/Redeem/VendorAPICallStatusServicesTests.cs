using System;
using Beis.Htg.VendorSme.Database.Models;
using Moq;
using System.Text.Json;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Api.Voucher.Tests.Redeem
{
    public class VendorAPICallStatusServicesTests
    {
        private VendorAPICallStatusServices _vendorApiCallStatusServices;
        private Mock<IVendorAPICallStatusRepository> _vendorApiCallStatusRepository;

        [SetUp]

        public void Setup()
        {
            _vendorApiCallStatusRepository = new Mock<IVendorAPICallStatusRepository>();
            _vendorApiCallStatusServices = new(_vendorApiCallStatusRepository.Object);
        }

        [Test]
        public void VendorAPICallStatusServicesCreateLogRequestDetailsHappyTests()
        {
            var voucherUpdateRequest = setupTestData();
            var vendor_api_call_status = _vendorApiCallStatusServices.CreateLogRequestDetails(voucherUpdateRequest);

            Assert.AreEqual("voucherRedeem", vendor_api_call_status.api_called);
            Assert.NotNull(vendor_api_call_status.vendor_id);
            Assert.NotNull(vendor_api_call_status.request);
            Assert.NotNull(vendor_api_call_status.call_datetime);
        }
        
        [Test]
        public async Task VendorAPICallStatusServicesLogRequestDetailsHappyTests()
        {
            var vendorAPICallStatus = LogRequestDetailsSetupTestData();
            
            await _vendorApiCallStatusServices.LogRequestDetails(vendorAPICallStatus);
            
            _vendorApiCallStatusRepository.Verify(v => v.LogRequestDetails(It.IsAny<vendor_api_call_status>()));
        }

        private VoucherUpdateRequest setupTestData()
        {
            VoucherUpdateRequest voucherUpdateRequest = new VoucherUpdateRequest()
            {
                Registration = "12345",
                AccessCode = "12345",
                VoucherCode = "IvMBLZ2PhUVkmJHpAxle0Q"
            };

            return voucherUpdateRequest;
        }
        
        private vendor_api_call_status LogRequestDetailsSetupTestData()
        {
            var voucherUpdateRequest = new VoucherUpdateRequest();
            voucherUpdateRequest = setupTestData();

            var vendorApiCallStatus = new vendor_api_call_status()
            {
                call_id = 12345,
                vendor_id = new[] {Convert.ToInt64(voucherUpdateRequest.Registration.Substring(1, voucherUpdateRequest.Registration.Length -1))},
                api_called = "VoucherRedeem",
                call_datetime = DateTime.Now,
                request = JsonSerializer.Serialize(voucherUpdateRequest)
            };

            _vendorApiCallStatusRepository.Setup(x => x.LogRequestDetails(It.IsAny<vendor_api_call_status>()));
            
            return vendorApiCallStatus;

        }


    }

}