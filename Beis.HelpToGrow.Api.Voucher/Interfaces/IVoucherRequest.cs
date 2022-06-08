namespace Beis.HelpToGrow.Api.Voucher.Interfaces
{
    public interface IVoucherRequest
    {
        string Registration { get; set; }
        string AccessCode { get; set; }
    }
}
