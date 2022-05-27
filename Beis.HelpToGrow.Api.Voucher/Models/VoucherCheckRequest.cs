
namespace Beis.HelpToGrow.Api.Voucher.Models
{
    public class VoucherCheckRequest : IVoucherRequest
    {
        [Required(ErrorMessage = "registration is required")]
        //  [MinLength(1, ErrorMessage = "registration is required")]
        [MaxLength(50, ErrorMessage = "registration cannot be more than 50 characters")]
        public string Registration { get; set; }
        [Required(ErrorMessage = "accessCode is required")]
        // [MinLength(1, ErrorMessage = "accessCode is required")]
        [MaxLength(50, ErrorMessage = "AccessCode cannot be more than 50 characters")]
        public string AccessCode { get; set; }
        [Required(ErrorMessage = "voucherCode is required")]
        //  [MinLength(1, ErrorMessage = "voucherCode is required")]
        [MaxLength(256, ErrorMessage = "VoucherCode cannot be more than 256 characters")]
        public string VoucherCode { get; set; }

        [Required(ErrorMessage = "CancellationReason is required")]
        //[MinLength(1, ErrorMessage = "CancellationReason is required")]
        [MaxLength(500, ErrorMessage = "CancellationReason cannot be more than 500 characters")]
        public string CancellationReason { get; set; }

        [Required(ErrorMessage = "CancellationDate is required")]
        public DateTime CancellationDate { get; set; }

        [Required(ErrorMessage = "ContractStartDate is required")]
        public DateTime ContractStartDate { get; set; }
    }
}