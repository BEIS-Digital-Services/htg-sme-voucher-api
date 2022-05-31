
namespace Beis.HelpToGrow.Api.Voucher.Models
{
    public class VoucherCheckResponse
    {
        public string Status { set; get; }
        public int ErrorCode { set; get; }
        public string Message { set; get; }
        public string VoucherCode { set; get; }
        public string AuthorisationCode { set; get; }
        public string Vendor { set; get; }
        public string ProductSku { set; get; }
        public string ProductName { set; get; }
        public string LicenceTo { set; get; }
        public string SmeEmail { set; get; }
        public string PurchaserName { set; get; }
        public decimal MaxDiscountAllowed { set; get; }
        public string Currency { set; get; }
        public string TokenExpiry { set; get; }
    }
}