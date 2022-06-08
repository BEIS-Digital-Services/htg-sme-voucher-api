
namespace Beis.HelpToGrow.Api.Voucher.Models
{
    public class VoucherGenerationResponse
    {
        public string Registration { get; set; }
        public string ProductSku { get; set; }
        public string VoucherCode { get; set; }
        public decimal VoucherBalance { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public int ErrorCode { get; set; }

    }
}