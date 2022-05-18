
namespace Beis.HelpToGrow.Api.Voucher.Models
{
    public class VoucherCheckRequest
    {
        [Required(ErrorMessage = "registration is required")]
        [MinLength(1, ErrorMessage = "registration is required")]
        public string registration { get; set; }
        [Required(ErrorMessage = "accessCode is required")]
        [MinLength(1, ErrorMessage = "accessCode is required")]
        public string accessCode { get; set; }
        [Required(ErrorMessage = "voucherCode is required")]
        [MinLength(1, ErrorMessage = "voucherCode is required")]
        public string voucherCode { get; set; }
    }
}