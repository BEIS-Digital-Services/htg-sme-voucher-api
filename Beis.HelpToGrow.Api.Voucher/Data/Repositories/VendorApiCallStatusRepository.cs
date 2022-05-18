
namespace Beis.HelpToGrow.Api.Data.Repositories
{
    public class VendorApiCallStatusRepository: IVendorAPICallStatusRepository
    {
        private readonly HtgVendorSmeDbContext _context;
        
        public VendorApiCallStatusRepository(HtgVendorSmeDbContext context)
        {
            _context = context;
        }

        public async Task LogRequestDetails(vendor_api_call_status vendorApiCallStatuses)
        {
           await _context.vendor_api_call_statuses.AddAsync(vendorApiCallStatuses);
           await _context.SaveChangesAsync();
        }
    }
}