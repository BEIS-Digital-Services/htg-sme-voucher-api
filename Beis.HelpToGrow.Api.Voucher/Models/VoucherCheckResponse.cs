
namespace Beis.HelpToGrow.Api.Voucher.Models
{
    public class VoucherCheckResponse
    {
        public string status { set; get; }
        public int errorCode { set; get; }
        public string message { set; get; }
        public string voucherCode { set; get; }
        public string authorisationCode { set; get; }
        public string vendor { set; get; }
        public string productSku { set; get; }
        public string productName { set; get; }
        public string licenceTo { set; get; }
        public string smeEmail { set; get; }
        public string purchaserName { set; get; }
        public decimal maxDiscountAllowed { set; get; }
        public string currency { set; get; }
        public string tokenExpiry { set; get; }
    }
}