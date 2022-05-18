
namespace Beis.HelpToGrow.Api.Voucher.Interfaces
{
    public interface IVoucherReconciliationService
    {
        public Task<VoucherReconciliationResponse> GetVoucherResponse(VoucherReconciliationRequest voucherRequest);
    }
}