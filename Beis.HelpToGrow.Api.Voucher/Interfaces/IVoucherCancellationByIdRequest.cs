namespace Beis.HelpToGrow.Api.Voucher.Interfaces
{
    public interface IVoucherCancellationByIdRequest
    {
        long VoucherId { get; set; }
    }
}