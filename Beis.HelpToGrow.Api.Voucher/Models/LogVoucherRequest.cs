namespace Beis.HelpToGrow.Api.Voucher.Models
{
    public class LogVoucherRequest : ILogVoucherRequest
    {
        public string ApiCalled { get; set; }
        public IVoucherRequest VoucherRequest { get; set; }
    }
}
