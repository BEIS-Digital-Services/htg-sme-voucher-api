
namespace Beis.HelpToGrow.Api.Voucher.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/voucherupdate")]
    public class VoucherRedeemController : ControllerBase
    {
        private readonly IVoucherRedeemService _voucherRedeemService;
        private ILogger<VoucherRedeemController> _logger;
        private IVendorAPICallStatusServices _vendorApiCallStatusServices;

        public VoucherRedeemController(ILogger<VoucherRedeemController> logger, IVoucherRedeemService voucherRedeemService, IVendorAPICallStatusServices vendorApiCallStatusServices)
        {
            _voucherRedeemService = voucherRedeemService;
            _logger = logger;
            _vendorApiCallStatusServices = vendorApiCallStatusServices;
        }
        
        /// <summary>
        /// Voucher update endpoint
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/voucherupdate
        ///     {
        ///        "registration": "12345",
        ///        "accessCode": "12345",
        ///        "voucherCode": "sH9ftM1rvm6N635qFVNdhg",
        ///        "authorisationCode": "179950987"
        ///     }
        ///
        /// </remarks>
        /// 

        [HttpPost]
        [ProducesResponseType(typeof(VoucherUpdateResponse), 200)]
        [ProducesResponseType(typeof(VoucherUpdateResponse), 400)]
        [ProducesResponseType(typeof(VoucherUpdateResponse), 500)]
        public async Task<ActionResult<VoucherUpdateResponse>> CheckVoucher([FromBody] VoucherUpdateRequest voucherUpdateRequest)
        {
            VoucherUpdateResponse voucherResponse;
            
            _logger.LogInformation("VoucherRedeemControllerRequest: {@VoucherUpdateRequest}", JsonSerializer.Serialize(voucherUpdateRequest));
            
            try
            {
                voucherResponse = await _voucherRedeemService.GetVoucherResponse(voucherUpdateRequest);

                if (voucherResponse.errorCode == 0)
                {
                    voucherResponse.status = "OK";
                    voucherResponse.message = "Successful check - proceed";
               
                    return Ok(voucherResponse);
                }        
                return StatusCode(400, voucherResponse);
            }
            catch (Exception e)
            {
                voucherResponse = new VoucherUpdateResponse
                {
                    status = "ERROR",
                    errorCode = 10,
                    message = "Unknown token",
                    voucherCode = voucherUpdateRequest.voucherCode
                };
                var vendorApiCallStatus = _vendorApiCallStatusServices.CreateLogRequestDetails(voucherUpdateRequest);
                vendorApiCallStatus.error_code = "500";
                vendorApiCallStatus.error_code = "10";
                await LogAPiCallStatus(vendorApiCallStatus, voucherResponse);
                if (e.Message.Length > 0)
                {
                     _logger.LogInformation("Error message {@message}" , e.Message);
                }
                _logger.LogInformation("VoucherRedeemControllerResponse: {@voucherResponse}", JsonSerializer.Serialize(voucherResponse));
                return StatusCode(500, voucherResponse);
            }                        
        }
        
        private async Task LogAPiCallStatus(vendor_api_call_status vendor_api_call_status, VoucherUpdateResponse voucherResponse)
        {
            vendor_api_call_status.result = JsonSerializer.Serialize(voucherResponse);
            await _vendorApiCallStatusServices.LogRequestDetails(vendor_api_call_status);
        }
    }
}