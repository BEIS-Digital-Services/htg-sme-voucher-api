
namespace Beis.HelpToGrow.Api.Voucher.Models
{
    public class VoucherReconciliationResponse
    {
        public string Status { get; set; }
        public int ErrorCode { get; set; }
        public string Message { get; set; }
        public List<VoucherReconciliationReport> ReconciliationReport { get; set; }

    }
}