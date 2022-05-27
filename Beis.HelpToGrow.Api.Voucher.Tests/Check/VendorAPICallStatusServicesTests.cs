using System;
using System.Text.Json;
using Moq;
using NUnit.Framework;

namespace Beis.HelpToGrow.Api.Voucher.Tests.Check
{
    [TestFixture]
    
    public class VendorAPICallStatusServicesTests
    {
        private VendorAPICallStatusServices _vendorApiCallStatusServices;
        private Mock<IVendorAPICallStatusRepository> _vendorApiCallStatusRepository;

        [SetUp]

        public void Setup()
        {
            _vendorApiCallStatusRepository = new Mock<IVendorAPICallStatusRepository>();
            _vendorApiCallStatusServices = new VendorAPICallStatusServices(_vendorApiCallStatusRepository.Object);
        }

        [Test]

        public void VendorAPICallStatusServicesCreateLogRequestDetailsHappyTests()
        {
            var voucherRequest = setupTestData();
            var vendor_api_call_status = _vendorApiCallStatusServices.CreateLogRequestDetails(voucherRequest);
            
            Assert.AreEqual("voucherCheck", vendor_api_call_status.api_called);
            Assert.NotNull(vendor_api_call_status.vendor_id);
            Assert.NotNull(vendor_api_call_status.request);
            Assert.NotNull(vendor_api_call_status.call_datetime);

        }

        [Test]

        public void VendorAPICallStatusServicesLogRequestDetailsHappyTests()
        {
            var vendorAPICallStatus = logRequestDetailsSetupTestData();
            
            _vendorApiCallStatusServices.LogRequestDetails(vendorAPICallStatus);
            
            _vendorApiCallStatusRepository.Verify(v => v.LogRequestDetails(It.IsAny<vendor_api_call_status>()));
        }

        private VoucherCheckRequest setupTestData()
        {
            VoucherCheckRequest voucherRequest = new VoucherCheckRequest()
            {
                Registration = "12345",
                AccessCode = "12345",
                VoucherCode = "IvMBLZ2PhUVkmJHpAxle0Q"
            };

            return voucherRequest;
        }
        
        private vendor_api_call_status logRequestDetailsSetupTestData()
        {
            var voucherRequest = new VoucherCheckRequest();
            voucherRequest = setupTestData();

            var vendorApiCallStatus = new vendor_api_call_status()
            {
                call_id = 12345,
                vendor_id = new[] {Convert.ToInt64(voucherRequest.Registration.Substring(1, voucherRequest.Registration.Length -1))},
                api_called = "VoucherCheck",
                call_datetime = DateTime.Now,
                request = JsonSerializer.Serialize(voucherRequest)
            };

            _vendorApiCallStatusRepository.Setup(x => x.LogRequestDetails(It.IsAny<vendor_api_call_status>()));
            
            return vendorApiCallStatus;

        }
        
        
    }
    
    
}