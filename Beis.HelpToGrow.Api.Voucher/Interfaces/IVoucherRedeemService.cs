
namespace Beis.HelpToGrow.Api.Voucher.Interfaces
{
    public interface IVoucherRedeemService
    {
        public Task<VoucherUpdateResponse> GetVoucherResponse(VoucherUpdateRequest voucherRequest);
    }
}