
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Beis.HelpToGrow.Api.Voucher.Controllers
{
    [Route("api/v{version:apiVersion}/paymentcancellation")]
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
        [ApiExplorerSettings(IgnoreApi = true)]
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

                voucherResponse = this.GetVoucherResponse(cancellationResult, cancellationRequest.VoucherCode);
            }
            catch (Exception e)
            {
                voucherResponse = new VoucherCancellationResponse
                {
                    Status = "ERROR",
                    ErrorCode = 40,
                    Message = "Unknown Error " + e.Message,
                    VoucherCode = cancellationRequest.VoucherCode
                };

                _logger.LogInformation("VoucherCancellationControllerResponse: {voucherResponse}", JsonSerializer.Serialize(voucherResponse));
                var vendor_api_call_status = _vendorApiCallStatusServices.CreateLogRequestDetails(new LogVoucherRequest
                {
                    ApiCalled = "voucherCancellation",
                    VoucherRequest = cancellationRequest
                });
                
                vendor_api_call_status.error_code = "500";
                await logAPiCallStatus(vendor_api_call_status, voucherResponse);

                return StatusCode(500, voucherResponse);
            }

            if (voucherResponse.ErrorCode != 0)
            {
                return StatusCode(400, voucherResponse);
            }

            return voucherResponse;
        }

        [HttpPost] 
        [Route("cancelbyid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(VoucherCancellationResponse), 200)]
        [ProducesResponseType(typeof(VoucherCancellationResponse), 400)]
        [ProducesResponseType(typeof(VoucherCancellationResponse), 500)]
        public async Task<ActionResult<VoucherCancellationResponse>> CancelById(VoucherCancellationByIdRequest cancellationRequest)
        {
            _logger.LogInformation("VoucherCancellationControllerRequest-CancelById: {cancellationRequest}", JsonSerializer.Serialize(cancellationRequest));
            VoucherCancellationResponse voucherResponse;
            try
            {
                var cancellationResult =
                    await _voucherCancellationService.CancelVoucherById(cancellationRequest.VoucherId);

                voucherResponse = this.GetVoucherResponse(cancellationResult, $"Id:{cancellationRequest.VoucherId}");
            }
            catch (Exception e)
            {
                voucherResponse = new VoucherCancellationResponse
                {
                    Status = "ERROR",
                    ErrorCode = 40,
                    Message = "Unknown Error " + e.Message,
                    VoucherCode = "Id:"+cancellationRequest.VoucherId
                };

                _logger.LogInformation("VoucherCancellationControllerResponse: {voucherResponse}", JsonSerializer.Serialize(voucherResponse));
                var vendor_api_call_status = _vendorApiCallStatusServices.CreateLogRequestDetails(new LogVoucherRequest
                {
                    ApiCalled = "voucherCancellation",
                    VoucherRequest = cancellationRequest
                });
                
                vendor_api_call_status.error_code = "500";
                await logAPiCallStatus(vendor_api_call_status, voucherResponse);

                return StatusCode(500, voucherResponse);
            }

            if (voucherResponse.ErrorCode != 0)
            {
                return StatusCode(400, voucherResponse);
            }

            return voucherResponse;
        }

        private VoucherCancellationResponse GetVoucherResponse(CancellationResponse cancellationResult, string voucherCodeValue)
        {
            VoucherCancellationResponse voucherResponse;
            
                 switch (cancellationResult)
                {
                    case CancellationResponse.SuccessfullyCancelled:
                        {
                            voucherResponse = new VoucherCancellationResponse
                            {
                                Status = "OK",
                                ErrorCode = 0,
                                Message = "Successfully cancelled",
                                VoucherCode = voucherCodeValue
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
                                VoucherCode = voucherCodeValue
                            };
                            return voucherResponse;
                        }
                    case CancellationResponse.FreeTrialExpired: 
                        {
                            voucherResponse = new VoucherCancellationResponse
                            {
                                Status = "OK",
                                ErrorCode = 0,
                                Message = "Voucher already cancelled. SME cannot reapply",
                                VoucherCode = voucherCodeValue
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
                                VoucherCode = voucherCodeValue
                            };
                            return voucherResponse;
                        }                    
                    case CancellationResponse.UnknownVoucherCode:                    
                    case CancellationResponse.TokenNotFound:
                        {
                            voucherResponse = new VoucherCancellationResponse
                            {
                                Status = "ERROR",
                                ErrorCode = 10,
                                Message = "Unknown Token",
                                VoucherCode = voucherCodeValue
                            };
                            return voucherResponse;
                        }                   
                    case CancellationResponse.UnknownVendorRegistration:                   
                        {
                            voucherResponse = new VoucherCancellationResponse
                            {
                                Status = "ERROR",
                                ErrorCode = 20,
                                Message = "Unknown Vendor",
                                VoucherCode = voucherCodeValue
                            };
                            return voucherResponse;
                        }
                    case CancellationResponse.UnknownVendorAccessCode:
                        {
                            voucherResponse = new VoucherCancellationResponse
                            {
                                Status = "ERROR",
                                ErrorCode = 30,
                                Message = "Unknown Access Code",
                                VoucherCode = voucherCodeValue
                            };
                            return voucherResponse;
                        }
                    case CancellationResponse.UnknownError:
                    default:
                        {
                            voucherResponse = new VoucherCancellationResponse
                            {
                                Status = "ERROR",
                                ErrorCode = 40,
                                Message = "Unknown Error",
                                VoucherCode = voucherCodeValue
                            };
                            return voucherResponse;
                        }
                }
        }

        private async Task logAPiCallStatus(vendor_api_call_status vendor_api_call_status, VoucherCancellationResponse voucherResponse)
        {
            vendor_api_call_status.result = JsonSerializer.Serialize(voucherResponse);
            await _vendorApiCallStatusServices.LogRequestDetails(vendor_api_call_status);
        }
    }
}
