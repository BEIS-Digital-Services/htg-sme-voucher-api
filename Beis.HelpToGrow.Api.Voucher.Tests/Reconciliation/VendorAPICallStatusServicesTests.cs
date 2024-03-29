using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Api.Voucher.Tests.Reconciliation
{
    public class VendorAPICallStatusServicesTests
    {
        private IVendorAPICallStatusServices _vendorApiCallStatusServices;
        private Mock<IVendorAPICallStatusRepository> _vendorApiCallStatusRepository;

        [SetUp]

        public void Setup()
        {
            _vendorApiCallStatusRepository = new Mock<IVendorAPICallStatusRepository>();

            
            _vendorApiCallStatusRepository.Setup(x => x.LogRequestDetails(It.IsAny<vendor_api_call_status>())).Returns(Task.CompletedTask);

            //_vendorApiCallStatusServices = new(_vendorApiCallStatusRepository.Object);
            _vendorApiCallStatusServices = new VendorAPICallStatusServices(_vendorApiCallStatusRepository.Object);

        }

        [Test]
        public void VendorAPICallStatusServicesCreateLogRequestDetailsHappyTests()
        {
            var VoucherReconciliationRequest = setupTestData();

            var vendorApiCallStatus = _vendorApiCallStatusServices.CreateLogRequestDetails(new LogVoucherRequest
            {
                ApiCalled = "voucherReconciliation",
                VoucherRequest = VoucherReconciliationRequest
            });

            Assert.AreEqual("voucherReconciliation", vendorApiCallStatus.api_called);
            Assert.NotNull(vendorApiCallStatus.vendor_id);
            Assert.NotNull(vendorApiCallStatus.request);
            Assert.NotNull(vendorApiCallStatus.call_datetime);
        }
        
        [Test]
        public async Task  VendorAPICallStatusServicesLogRequestDetailsHappyTests()
        {
            var vendorAPICallStatus = logRequestDetailsSetupTestData();
            
            await _vendorApiCallStatusServices.LogRequestDetails(vendorAPICallStatus);
            
            _vendorApiCallStatusRepository.Verify(v => v.LogRequestDetails(It.IsAny<vendor_api_call_status>()));
        }

        private VoucherReconciliationRequest setupTestData()
        {
            VoucherReconciliationRequest voucherReconciliationRequest = new VoucherReconciliationRequest()
            {
                Registration = "12345",
                AccessCode = "12345",
                ReconciliationDate = DateTime.Parse("2022-01-10")
            };

            return voucherReconciliationRequest;
        }
        
        private vendor_api_call_status logRequestDetailsSetupTestData()
        {
            var voucherReconciliationRequest = new VoucherReconciliationRequest();
            voucherReconciliationRequest = setupTestData();

            var vendorApiCallStatus = new vendor_api_call_status()
            {
                call_id = 12345,
                vendor_id = new[] {Convert.ToInt64(voucherReconciliationRequest.Registration.Substring(1, voucherReconciliationRequest.Registration.Length -1))},
                api_called = "VoucherReconciliation",
                call_datetime = DateTime.Now,
                request = JsonSerializer.Serialize(voucherReconciliationRequest),
                result = JsonSerializer.Serialize(new VoucherReconciliationResponse()),
            };

            
            return vendorApiCallStatus;

        }
        
    }
}