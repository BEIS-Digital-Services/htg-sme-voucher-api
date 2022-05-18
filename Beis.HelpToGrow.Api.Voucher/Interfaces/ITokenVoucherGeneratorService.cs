
namespace Beis.HelpToGrow.Api.Voucher.Interfaces
{
    public interface ITokenVoucherGeneratorService
    {
        Task<VoucherGenerationResponse> GenerateVoucher(VoucherGenerationRequest voucherGenerationRequest);
    }
}