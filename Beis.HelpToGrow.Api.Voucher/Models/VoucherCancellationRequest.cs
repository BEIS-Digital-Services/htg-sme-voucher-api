using System.Text.Json.Serialization;

namespace Beis.HelpToGrow.Api.Voucher.Models
{
    /// <summary>
    ///        "registration":"R12345",
    ///        "accessCode":"SDFgnYTrVF",
    ///        "voucherCode": "JUysert6uYYbdWKJHtrsRQ34",
    ///        "cancellationReason"Reason for cancellation",
    ///        "cancellationDate":"15/03/2020 14:37”,
    ///        "contractStartDate":"15/02/2020 14:37”
    /// </summary>
    public class VoucherCancellationRequest : IVoucherRequest
    {
        [JsonIgnore]
        public int VoucherId { get; set; }
        
        [Required(ErrorMessage = "Registration is required")]
        [MaxLength(50, ErrorMessage = "Registration cannot be more than 50 characters")]
        public string Registration { get; set; }
        [Required(ErrorMessage = "AccessCode is required")]
        [MaxLength(50, ErrorMessage = "AccessCode cannot be more than 50 characters")]
        public string AccessCode { get; set; }
        [Required(ErrorMessage = "VoucherCode is required")]
        [MaxLength(256, ErrorMessage = "VoucherCode cannot be more than 256 characters")]
        public string VoucherCode { get; set; }

        [Required(ErrorMessage = "CancellationReason is required")]
        [MaxLength(500, ErrorMessage = "CancellationReason cannot be more than 500 characters")]
        public string CancellationReason { get; set; }

        [Required(ErrorMessage = "CancellationDate is required")]
        public DateTime CancellationDate { get; set; }

        [Required(ErrorMessage = "ContractStartDate is required")]
        public DateTime ContractStartDate { get; set; }

    }
}
