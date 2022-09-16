namespace Beis.HelpToGrow.Api.Voucher.Interfaces
{
    public interface IVoucherRequest
    {
        int VoucherId { get; set; }
        string Registration { get; set; }
        string AccessCode { get; set; }
    }
}
