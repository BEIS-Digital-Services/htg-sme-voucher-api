
namespace Beis.HelpToGrow.Api.Voucher.Models
{
    public class VoucherGenerationRequest
    {
        [Required(ErrorMessage = "Registration is required")]
        [MinLength(1, ErrorMessage = "Registration is required")]
        public string Registration { get; set; }
        [Required(ErrorMessage = "ProductSku is required")]
        [MinLength(1, ErrorMessage = "ProductSku is required")]
        public string ProductSku { get; set; }
    }
}