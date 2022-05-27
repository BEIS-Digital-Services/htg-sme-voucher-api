
namespace Beis.HelpToGrow.Api.Voucher.Models
{
    public class VoucherReconciliationRequest : IVoucherRequest
    {
        public string Registration { get; set; }
        public string AccessCode { get; set; }
        public DateTime ReconciliationDate { get; set; }
        public DailySales DailySales { get; set; }

    }
}