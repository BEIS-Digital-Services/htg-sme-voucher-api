using System.Text.Json.Serialization;

namespace Beis.HelpToGrow.Api.Voucher.Models
{
    public class VoucherReconciliationRequest : IVoucherRequest
    {
        [JsonIgnore]
        public int VoucherId { get; set; }
        public string Registration { get; set; }
        public string AccessCode { get; set; }
        public DateTime ReconciliationDate { get; set; }
        public DailySales DailySales { get; set; }

    }
}