
namespace Beis.HelpToGrow.Api.Voucher.Interfaces
{
    public interface IVendorCompanyRepository
    {
        vendor_company GetVendorCompanyByRegistration(string registrationId);
    }
}