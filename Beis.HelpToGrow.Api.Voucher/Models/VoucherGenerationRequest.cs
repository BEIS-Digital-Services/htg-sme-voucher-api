using System.Text.Json.Serialization;

namespace Beis.HelpToGrow.Api.Voucher.Models
{
    public class VoucherGenerationRequest: IVoucherRequest
    {
        [JsonIgnore]
        public int VoucherId { get; set; }

        [Required(ErrorMessage = "Registration is required")]
        [MinLength(1, ErrorMessage = "Registration is required")]
        public string Registration { get; set; }

        [JsonIgnore]
        public string AccessCode { get; set; }

        [Required(ErrorMessage = "ProductSku is required")]
        [MinLength(1, ErrorMessage = "ProductSku is required")]
        public string ProductSku { get; set; }
    }
}