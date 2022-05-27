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
        [Required(ErrorMessage = "registration is required")]
        [MinLength(1, ErrorMessage = "registration is required")]
        public string Registration { get; set; }
        [Required(ErrorMessage = "accessCode is required")]
        [MinLength(1, ErrorMessage = "accessCode is required")]
        public string AccessCode { get; set; }
        [Required(ErrorMessage = "voucherCode is required")]
        [MinLength(1, ErrorMessage = "voucherCode is required")]
        public string VoucherCode { get; set; }
        public string CancellationReason { get; set; }

        public DateTime CancellationDate { get; set;}
        public DateTime ContractStartDate { get; set;}

    }
}
