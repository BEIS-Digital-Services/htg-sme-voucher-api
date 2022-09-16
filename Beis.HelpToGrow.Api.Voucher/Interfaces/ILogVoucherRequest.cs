namespace Beis.HelpToGrow.Api.Voucher.Interfaces
{
    public interface ILogVoucherRequest
    {
        string ApiCalled { get; set; }

        IVoucherRequest VoucherRequest { get; set; }
    }
}
