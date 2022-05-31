
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Beis.HelpToGrow.Api.Voucher.Controllers
{


    [Route("api/v{version:apiVersion}paymentcancellation")]
    [ApiController]
    public class VoucherCancellationController : ControllerBase
    {
        private readonly ILogger<VoucherCancellationController> _logger;
        private readonly IVoucherCancellationService _voucherCancellationService;
        private IVendorAPICallStatusServices _vendorApiCallStatusServices;

        public VoucherCancellationController(ILogger<VoucherCancellationController> logger, IVoucherCancellationService voucherCancellationService, IVendorAPICallStatusServices vendorApiCallStatusServices)
        {
            _logger = logger;
            _voucherCancellationService = voucherCancellationService;
            _vendorApiCallStatusServices = vendorApiCallStatusServices;
        }


        /// <remarks>
        /// 
        /// Sample request:
        /// 
        ///     POST api/v1.0/paymentcancellation
        ///     {
        ///        "registration":"R12345",
        ///        "accessCode":"SDFgnYTrVF",
        ///        "voucherCode": "JUysert6uYYbdWKJHtrsRQ34",
        ///        "cancellationReason"Reason for cancellation",
        ///        "cancellationDate":"15/03/2020 14:37”,
        ///        "contractStartDate":"15/02/2020 14:37”
        ///     }
        /// </remarks>

        // POST api/<VoucherCancellationController>

        [HttpPost]
        [ProducesResponseType(typeof(VoucherCancellationResponse), 200)]
        [ProducesResponseType(typeof(VoucherCancellationResponse), 400)]
        [ProducesResponseType(typeof(VoucherCancellationResponse), 500)]
        public async Task<ActionResult<VoucherCancellationResponse>> Post(VoucherCancellationRequest cancellationRequest)
        {
            _logger.LogInformation("VoucherCancellationControllerRequest: {cancellationRequest}", JsonSerializer.Serialize(cancellationRequest));
            VoucherCancellationResponse voucherResponse;
            try
            {
                var cancellationResult = await _voucherCancellationService.CancelVoucherFromVoucherCode(cancellationRequest.VoucherCode,cancellationRequest.Registration, cancellationRequest.AccessCode);
                switch (cancellationResult)
                {
                    case CancellationResponse.SuccessfullyCancelled:
                        {
                            voucherResponse = new VoucherCancellationResponse
                            {
                                Status = "OK",
                                ErrorCode = 0,
                                Message = "Successfully cancelled",
                                VoucherCode = cancellationRequest.VoucherCode
                            };
                            return voucherResponse;
                        }
                    case CancellationResponse.AlreadyCancelled:
                        {
                            voucherResponse = new VoucherCancellationResponse
                            {
                                Status = "OK",
                                ErrorCode = 0,
                                Message = "Voucher already cancelled",
                                VoucherCode = cancellationRequest.VoucherCode
                            };
                            return voucherResponse;
                        }
                    case CancellationResponse.TokenExpired:
                        {
                            voucherResponse = new VoucherCancellationResponse
                            {
                                Status = "OK",
                                ErrorCode = 0,
                                Message = "Voucher already expired",
                                VoucherCode = cancellationRequest.VoucherCode
                            };
                            return voucherResponse;

                        }
                    case CancellationResponse.UnknownVoucherCode:
                    case CancellationResponse.UnknownError:
                    case CancellationResponse.TokenNotFound:

                        {
                            voucherResponse = new VoucherCancellationResponse
                            {
                                Status = "ERROR",
                                ErrorCode = 10,
                                Message = "Unknown Voucher",
                                VoucherCode = cancellationRequest.VoucherCode
                            };
                            return voucherResponse;
                        }
                    case CancellationResponse.FreeTrialExpired: // todo - discuss whethere this should still cancel the voucher.
                        {
                            voucherResponse = new VoucherCancellationResponse
                            {
                                Status = "ERROR",
                                ErrorCode = 10,
                                Message = "Free trial expired. SME cannot reapply.",
                                VoucherCode = cancellationRequest.VoucherCode
                            };
                            return voucherResponse;
                        }
                    case CancellationResponse.UnknownVendorRegistration:
                    case CancellationResponse.UnknownVendorAccessCode:
                        {
                            voucherResponse = new VoucherCancellationResponse
                            {
                                Status = "ERROR",
                                ErrorCode = 20,
                                Message = "Unknown Vendor",
                                VoucherCode = cancellationRequest.VoucherCode
                            };
                            return voucherResponse;
                        }
                    default:
                        {
                            voucherResponse = new VoucherCancellationResponse
                            {
                                Status = "ERROR",
                                ErrorCode = 10,
                                Message = "Unknown Voucher",
                                VoucherCode = cancellationRequest.VoucherCode
                            };
                            return voucherResponse;
                        }

                }

            }
            catch (Exception e)
            {
                voucherResponse = new VoucherCancellationResponse
                {
                    Status = "ERROR",
                    ErrorCode = 10,
                    Message = "Unknown token " + e.Message,

                    VoucherCode = cancellationRequest.VoucherCode
                };

                _logger.LogInformation("VoucherCancellationControllerResponse: {voucherResponse}", JsonSerializer.Serialize(voucherResponse));
                var vendor_api_call_status = _vendorApiCallStatusServices.CreateLogRequestDetails(cancellationRequest);
                vendor_api_call_status.error_code = "500";
                await logAPiCallStatus(vendor_api_call_status, voucherResponse);

                return StatusCode(500, voucherResponse);
            }

            return voucherResponse;
        }

        private async Task logAPiCallStatus(vendor_api_call_status vendor_api_call_status, VoucherCancellationResponse voucherResponse)
        {
            vendor_api_call_status.result = JsonSerializer.Serialize(voucherResponse);
            await _vendorApiCallStatusServices.LogRequestDetails(vendor_api_call_status);
        }


    }
}
