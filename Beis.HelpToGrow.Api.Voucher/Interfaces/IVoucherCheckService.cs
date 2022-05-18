
namespace Beis.HelpToGrow.Api.Voucher.Interfaces
{
    public interface IVoucherCheckService
    {
        public Task<VoucherCheckResponse> GetVoucherResponse(VoucherCheckRequest voucherRequest);
    }
}