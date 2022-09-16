
namespace Beis.HelpToGrow.Api.Voucher.Interfaces
{ 
    public interface IVendorAPICallStatusServices
    {
        vendor_api_call_status CreateLogRequestDetails(ILogVoucherRequest logVoucherRequest);
        Task LogRequestDetails(vendor_api_call_status vendorApiCallStatuses);
        
        
        
    }
}