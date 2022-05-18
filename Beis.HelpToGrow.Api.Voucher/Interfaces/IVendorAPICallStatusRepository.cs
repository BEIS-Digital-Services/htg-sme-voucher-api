
namespace Beis.HelpToGrow.Api.Voucher.Interfaces
{
    public interface IVendorAPICallStatusRepository
    {
        Task LogRequestDetails(vendor_api_call_status vendorApiCallStatuses);
    }
}