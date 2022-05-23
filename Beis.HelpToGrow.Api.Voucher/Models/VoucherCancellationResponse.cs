namespace Beis.HelpToGrow.Api.Voucher.Models
{
    public class VoucherCancellationResponse
    {
        public string status { get; set; }
        public int errorCode { get; set; }
        public string message { get; set; }
        public string voucherCode { get; set; }
    }
}
