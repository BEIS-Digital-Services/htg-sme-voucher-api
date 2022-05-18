
namespace Beis.HelpToGrow.Api.Voucher.Interfaces
{ 
    public interface IVendorAPICallStatusServices
    {
        vendor_api_call_status CreateLogRequestDetails(VoucherCheckRequest voucherRequest);
        vendor_api_call_status CreateLogRequestDetails(VoucherUpdateRequest voucherUpdateRequest);
        vendor_api_call_status CreateLogRequestDetails(VoucherReconciliationRequest voucherReconciliationRequest);
        Task LogRequestDetails(vendor_api_call_status vendorApiCallStatuses);
        
    }
}