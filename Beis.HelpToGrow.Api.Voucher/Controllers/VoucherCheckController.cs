
namespace Beis.HelpToGrow.Api.Voucher.Controllers
{
    [ApiController]
    [Route("api/vouchercheck")]
    public class VoucherCheckController : ControllerBase
    {
        private readonly IVoucherCheckService _voucherCheckService;
        private ILogger<VoucherCheckController> _logger;
        private IVendorAPICallStatusServices _vendorApiCallStatusServices;

        public VoucherCheckController(ILogger<VoucherCheckController> logger, IVoucherCheckService voucherCheckService, IVendorAPICallStatusServices vendorApiCallStatusServices)
        {
            _voucherCheckService = voucherCheckService;
            _logger = logger;
            _vendorApiCallStatusServices = vendorApiCallStatusServices;
        }

        /// <summary>
        /// Voucher check endpoint 
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/vouchercheck
        ///     {
        ///        "registration": 12345,
        ///        "accessCode": "12345",
        ///        "voucherCode": "sH9ftM1rvm6N635qFVNdhg"
        ///     }
        ///
        /// </remarks>
        ///
        /// 
        [HttpPost]
        [ProducesResponseType(typeof(VoucherCheckResponse), 200)]
        [ProducesResponseType(typeof(VoucherCheckResponse), 400)]
        [ProducesResponseType(typeof(VoucherCheckResponse), 500)]
        public async Task<ActionResult<VoucherCheckResponse>> CheckVoucher([FromBody] VoucherCheckRequest voucherRequest)
        {
            _logger.LogInformation("VoucherCheckControllerRequest: {@VoucherRequest}", JsonSerializer.Serialize(voucherRequest));
            
            VoucherCheckResponse voucherResponse;
            try
            {
                voucherResponse = await _voucherCheckService.GetVoucherResponse(voucherRequest);
                if (voucherResponse.ErrorCode == 0)
                {
                    _logger.LogInformation("VoucherCheckControllerResponse: {@voucherResponse}", JsonSerializer.Serialize(voucherResponse));
                    return Ok(voucherResponse);
                }
                _logger.LogInformation("VoucherCheckControllerResponse: {@voucherResponse}", JsonSerializer.Serialize(voucherResponse));
                return StatusCode(400, voucherResponse);                
            }
            catch (Exception e)
            {
                 voucherResponse = new VoucherCheckResponse
                {
                    Status = "ERROR",
                    ErrorCode = 10,
                    Message = "Unknown token " + e.Message, 
                  
                    VoucherCode = voucherRequest.VoucherCode
                };
                 
                _logger.LogInformation("VoucherCheckControllerResponse: {@voucherResponse}", JsonSerializer.Serialize(voucherResponse));
                var vendor_api_call_status = _vendorApiCallStatusServices.CreateLogRequestDetails(new LogVoucherRequest
                {
                    ApiCalled = "voucherCheck",
                    VoucherRequest = voucherRequest
                });

                vendor_api_call_status.error_code = "500";
                await LogAPiCallStatus(vendor_api_call_status, voucherResponse);

                return StatusCode(500, voucherResponse);
            }                       
        }

        private async Task LogAPiCallStatus(vendor_api_call_status vendor_api_call_status, VoucherCheckResponse voucherResponse)
        {
            vendor_api_call_status.result = JsonSerializer.Serialize(voucherResponse);
            await _vendorApiCallStatusServices.LogRequestDetails(vendor_api_call_status);
        }
    }
}