
namespace Beis.HelpToGrow.Api.Voucher.Models
{
    public class VoucherReconciliationResponse
    {
        public string status { get; set; }
        public int errorCode { get; set; }
        public string message { get; set; }
        public List<VoucherReconciliationReport> reconciliationReport { get; set; }

    }
}