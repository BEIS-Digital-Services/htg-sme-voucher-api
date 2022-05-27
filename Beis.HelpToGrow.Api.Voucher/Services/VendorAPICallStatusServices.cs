
namespace Beis.HelpToGrow.Api.Voucher.Services
{
    public class VendorAPICallStatusServices: IVendorAPICallStatusServices
    {
        private IVendorAPICallStatusRepository _vendorApiCallStatusRepository;
        
        public VendorAPICallStatusServices(IVendorAPICallStatusRepository vendorApiCallStatusRepository)
        {
            _vendorApiCallStatusRepository = vendorApiCallStatusRepository;
        }

        public vendor_api_call_status CreateLogRequestDetails(IVoucherRequest voucherRequest)
        {
            var apiCalled = "Unknown";
            switch (voucherRequest)
            {
                case VoucherCheckRequest:
                    {
                        apiCalled = "voucherCheck";
                        break;
                    }
                case VoucherUpdateRequest:
                    {
                        apiCalled = "voucherRedeem";
                        break;
                    }
                case VoucherReconciliationRequest:
                    {
                        apiCalled = "voucherReconciliation";
                        break;
                    }
                case VoucherCancellationRequest:
                    {
                        apiCalled = "voucherCancellation";
                        break;
                    }
            }

            var apiCallStatus = new vendor_api_call_status
            {
                // call_id = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                vendor_id = new[] { Convert.ToInt64(voucherRequest.Registration.Substring(1, voucherRequest.Registration.Length - 1)) },
                api_called = apiCalled,
                call_datetime = DateTime.Now,
                request = JsonSerializer.Serialize(voucherRequest)
            };

            return apiCallStatus;
        }

        public vendor_api_call_status CreateLogRequestDetails(VoucherUpdateRequest voucherUpdateRequest)
        {
            var apiCallStatus = new vendor_api_call_status
            {
                // call_id = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                vendor_id = new[] {Convert.ToInt64(voucherUpdateRequest.Registration.Substring(1, voucherUpdateRequest.Registration.Length -1))},
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
                vendor_id = new[] { Convert.ToInt64(voucherRequest.Registration.Substring(1, voucherRequest.Registration.Length - 1)) },
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
                vendor_id = new[] { Convert.ToInt64(voucherReconciliationRequest.Registration.Substring(1, voucherReconciliationRequest.Registration.Length - 1)) },
                api_called = "voucherReconciliation",
                call_datetime = DateTime.Now,
                request = JsonSerializer.Serialize(voucherReconciliationRequest)
            };

            return apiCallStatus;
        }
    }
}