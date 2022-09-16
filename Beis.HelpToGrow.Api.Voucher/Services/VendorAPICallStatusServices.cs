
namespace Beis.HelpToGrow.Api.Voucher.Services
{
    public class VendorAPICallStatusServices: IVendorAPICallStatusServices
    {
        private IVendorAPICallStatusRepository _vendorApiCallStatusRepository;
        
        public VendorAPICallStatusServices(IVendorAPICallStatusRepository vendorApiCallStatusRepository)
        {
            _vendorApiCallStatusRepository = vendorApiCallStatusRepository;
        }
        
        public vendor_api_call_status CreateLogRequestDetails(ILogVoucherRequest logVoucherRequest)
        {

            var apiCallStatus = new vendor_api_call_status
            {
                // call_id = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                vendor_id = new[]
                {
                    string.IsNullOrEmpty(logVoucherRequest.VoucherRequest.Registration) 
                        ? -1 
                        : Convert.ToInt64(logVoucherRequest.VoucherRequest.Registration.Substring(
                            1, 
                            logVoucherRequest.VoucherRequest.Registration.Length - 1))
                },
                api_called = logVoucherRequest.ApiCalled ?? "Unknown",
                call_datetime = DateTime.Now,
                request = JsonSerializer.Serialize(logVoucherRequest.VoucherRequest, logVoucherRequest.VoucherRequest.GetType())
            };

            return apiCallStatus;
        }

        public async Task LogRequestDetails(vendor_api_call_status vendorApiCallStatuses)
        {
            await _vendorApiCallStatusRepository.LogRequestDetails(vendorApiCallStatuses);
        }
    }
}