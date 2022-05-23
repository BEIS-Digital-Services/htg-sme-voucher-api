
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Beis.HelpToGrow.Api.Voucher.Controllers
{
    [Route("api/vouchercancellation")]
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

        //// GET: api/<VoucherCancellationController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<VoucherCancellationController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<VoucherCancellationController>
        [HttpPost]
        public async Task<VoucherCancellationResponse> Post(VoucherCancellationRequest cancellationRequest)
        {
            return new VoucherCancellationResponse { };
        }


    }
}
