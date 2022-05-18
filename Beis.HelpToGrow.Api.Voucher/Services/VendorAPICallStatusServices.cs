
namespace Beis.HelpToGrow.Api.Voucher.Services
{
    public class VendorAPICallStatusServices: IVendorAPICallStatusServices
    {
        private IVendorAPICallStatusRepository _vendorApiCallStatusRepository;
        
        public VendorAPICallStatusServices(IVendorAPICallStatusRepository vendorApiCallStatusRepository)
        {
            _vendorApiCallStatusRepository = vendorApiCallStatusRepository;
        }
        
        public vendor_api_call_status CreateLogRequestDetails(VoucherUpdateRequest voucherUpdateRequest)
        {
            var apiCallStatus = new vendor_api_call_status
            {
                // call_id = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                vendor_id = new[] {Convert.ToInt64(voucherUpdateRequest.registration.Substring(1, voucherUpdateRequest.registration.Length -1))},
                api_called = "voucherRedeem",
                call_datetime = DateTime.Now,
                request = JsonSerializer.Serialize(voucherUpdateRequest)
            };
            
            return apiCallStatus;
        }


        public vendor_api_call_status CreateLogRequestDetails(VoucherCheckRequest voucherRequest)
        {
            var apiCallStatus = new vendor_api_call_status
            {
                // call_id = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                vendor_id = new[] { Convert.ToInt64(voucherRequest.registration.Substring(1, voucherRequest.registration.Length - 1)) },
                api_called = "voucherCheck",
                call_datetime = DateTime.Now,
                request = JsonSerializer.Serialize(voucherRequest)
            };

            return apiCallStatus;
        }

        public async Task LogRequestDetails(vendor_api_call_status vendorApiCallStatuses)
        {
            await _vendorApiCallStatusRepository.LogRequestDetails(vendorApiCallStatuses);
        }

        public vendor_api_call_status CreateLogRequestDetails(VoucherReconciliationRequest voucherReconciliationRequest)
        {
            var apiCallStatus = new vendor_api_call_status
            {
                // call_id = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                vendor_id = new[] { Convert.ToInt64(voucherReconciliationRequest.registration.Substring(1, voucherReconciliationRequest.registration.Length - 1)) },
                api_called = "voucherReconciliation",
                call_datetime = DateTime.Now,
                request = JsonSerializer.Serialize(voucherReconciliationRequest)
            };

            return apiCallStatus;
        }
    }
}