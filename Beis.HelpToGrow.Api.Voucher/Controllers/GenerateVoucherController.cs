
namespace Beis.HelpToGrow.Api.Voucher.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    //[Route("api/v{version:apiVersion}/generatevoucher")]
    [Route("api/generatevoucher")]
    public class GenerateVoucherController: ControllerBase
    {
        private readonly ITokenVoucherGeneratorService _tokenVoucherGeneratorService;
        private ILogger<VoucherCheckController> _logger;
        public GenerateVoucherController(ILogger<VoucherCheckController> logger, ITokenVoucherGeneratorService tokenVoucherGeneratorService)
        {
            _tokenVoucherGeneratorService = tokenVoucherGeneratorService;
            _logger = logger;
        }

        /// <summary>
        /// Voucher check endpoint. 
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/vouchercheck
        ///     {
        ///        "registration": 12345,
        ///        "productSku": "12345"
        ///     }
        ///
        /// </remarks>
        /// 
        [ServiceFilter(typeof(CanGenerateTestVoucherAttribute))]
        [HttpPost]
        public async Task<ActionResult<VoucherGenerationResponse>> GenerateVoucher(VoucherGenerationRequest voucherGenerationRequest)
        {
            _logger.LogInformation("VoucherCheckControllerRequest: {@VoucherRequest}", JsonSerializer.Serialize(voucherGenerationRequest));
            VoucherGenerationResponse response;
            try
            {
                response= await _tokenVoucherGeneratorService.GenerateVoucher(voucherGenerationRequest);
                if (response.ErrorCode == 0)
                {
                    _logger.LogInformation("VoucherCheckControllerResponse: {@voucherResponse}", JsonSerializer.Serialize(response));
                    return response;
                }
                return StatusCode(400, response);
            }
            catch (Exception e)
            {
                response =  new VoucherGenerationResponse()
                {
                    Status = "ERROR",
                    Registration = voucherGenerationRequest.Registration,
                    ProductSku = voucherGenerationRequest.ProductSku,
                    ErrorCode = 10,
                    Message = "Unknown ProductSKU or Vendor details"
                };
                _logger.LogError("VoucherCheckServiceResponse: {@VoucherResponse}, {@VErrorMessage}",JsonSerializer.Serialize(response), e.Message);
                
            }
            return StatusCode(500, response);
        }

    }
}