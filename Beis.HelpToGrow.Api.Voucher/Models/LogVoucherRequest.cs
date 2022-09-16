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
    public class LogVoucherRequest : ILogVoucherRequest
    {
        public string ApiCalled { get; set; }
        public IVoucherRequest VoucherRequest { get; set; }
    }
}
