using System.Text.Json.Serialization;

namespace Beis.HelpToGrow.Api.Voucher.Models
{
    public class VoucherCancellationByIdRequest : IVoucherRequest
    {
        [Required(ErrorMessage = "Voucher Id is required")]
        public int VoucherId { get; set; }

        [JsonIgnore]
        public string Registration { get; set; }
        [JsonIgnore]
        public string AccessCode { get; set; }
    }
}
