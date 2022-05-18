
namespace Beis.HelpToGrow.Api.Voucher.Interfaces
{
    public interface IVoucherGeneratorService
    {
        public Task<string> GenerateVoucher(vendor_company vendorCompany, enterprise enterprise, product product);
        public string GenerateSetCode(int length);
    }
}