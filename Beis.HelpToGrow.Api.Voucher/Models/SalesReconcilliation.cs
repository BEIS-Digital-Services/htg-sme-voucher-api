
namespace Beis.HelpToGrow.Api.Voucher.Models
{
    public class SalesReconcilliation
    {
        public string NotificationType { get; set; }
        public string VoucherCode { get; set; }
        public string AuthorisationCode { get; set; }
        public string ProductSku { get; set; }
        public string ProductName { get; set; }
        public string LicenceTo { get; set; }
        public string SmeEmail { get; set; }
        public string PurchaserName { get; set; }
        public decimal OneOffCosts { get; set; }
        public int NoOfLicences { get; set; }
        public decimal CostPerLicence { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountApplied { get; set; }
        public string Currency { get; set; }
        public decimal ContractTermInMonths { get; set; }
        public decimal TrialPeriodInMonths { get; set; }

    }
}