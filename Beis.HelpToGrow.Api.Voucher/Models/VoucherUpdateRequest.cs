
namespace Beis.HelpToGrow.Api.Voucher.Models
{
    public class VoucherUpdateRequest : IVoucherRequest
    {
        [Required(ErrorMessage = "Registration is required")]
        [MinLength(1, ErrorMessage = "Registration is required")]
        public string Registration { get; set; }
        [Required(ErrorMessage = "AccessCode is required")]
        [MinLength(1, ErrorMessage = "AccessCode is required")]
        public string AccessCode { get; set; }
        [Required(ErrorMessage = "VoucherCode is required")]
        [MinLength(1, ErrorMessage = "VoucherCode is required")]
        public string VoucherCode { get; set; }
        [Required(ErrorMessage = "AuthorisationCode is required")]
        [MinLength(1, ErrorMessage = "AuthorisationCode is required")]
        public string AuthorisationCode { get; set; }
    }
}