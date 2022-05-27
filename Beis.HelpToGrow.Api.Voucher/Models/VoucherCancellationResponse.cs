namespace Beis.HelpToGrow.Api.Voucher.Models
{
    public class VoucherCancellationResponse 
    {
        public string Status { get; set; }
        public int ErrorCode { get; set; }
        public string Message { get; set; }
        public string VoucherCode { get; set; }
    }
}
