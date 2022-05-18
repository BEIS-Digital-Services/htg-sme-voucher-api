
namespace Beis.HelpToGrow.Api.Voucher.Models
{
    public class VoucherReconciliationRequest
    {
        public string registration { get; set; }
        public string accessCode { get; set; }
        public DateTime reconciliationDate { get; set; }
        public DailySales dailySales { get; set; }

    }
}