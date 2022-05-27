
namespace Beis.HelpToGrow.Api.Voucher.Models
{
    public class VoucherUpdateRequest : IVoucherRequest
    {
        [Required(ErrorMessage = "registration is required")]
        [MinLength(1, ErrorMessage = "registration is required")]
        public string Registration { get; set; }
        [Required(ErrorMessage = "accessCode is required")]
        [MinLength(1, ErrorMessage = "accessCode is required")]
        public string AccessCode { get; set; }
        [Required(ErrorMessage = "voucherCode is required")]
        [MinLength(1, ErrorMessage = "voucherCode is required")]
        public string VoucherCode { get; set; }
        [Required(ErrorMessage = "authorisationCode is required")]
        [MinLength(1, ErrorMessage = "authorisationCode is required")]
        public string AuthorisationCode { get; set; }
    }
}