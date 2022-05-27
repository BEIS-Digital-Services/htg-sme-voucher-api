
namespace Beis.HelpToGrow.Api.Voucher.Services
{
    public class TokenVoucherGeneratorService : ITokenVoucherGeneratorService
    {
        private readonly IProductRepository _productRepository;
        private readonly IVendorCompanyRepository _vendorCompanyRepository;
        private readonly IVoucherGenerationService _voucherGeneratorService;

        public TokenVoucherGeneratorService(IProductRepository productRepository,
            IVendorCompanyRepository vendorCompanyRepository,
            IVoucherGenerationService voucherGeneratorService)
        {
            _productRepository = productRepository;
            _vendorCompanyRepository = vendorCompanyRepository;
            _voucherGeneratorService = voucherGeneratorService;
        }
        private static decimal tokenBalance => 5000M;
        public async Task<VoucherGenerationResponse> GenerateVoucher(VoucherGenerationRequest voucherGenerationRequest)
        {

            var vendorCompany = _vendorCompanyRepository.GetVendorCompanyByRegistration(voucherGenerationRequest.registration);
            if (vendorCompany == null)
            {
                return new VoucherGenerationResponse()
                {
                    status = "ERROR",
                    message = "Company registration not found."
                };
            }
            var product = _productRepository.GetProductBySku(voucherGenerationRequest.productSku, vendorCompany.vendorid);

            if (product == null)
            {
                return new VoucherGenerationResponse()
                {
                    status = "ERROR",
                    message = "Product not found."
                };
            }
            var enterprise = new enterprise()
            {
                enterprise_id = 1
            };

            var voucherCode = await _voucherGeneratorService.GenerateVoucher(vendorCompany, enterprise, product);

            return new VoucherGenerationResponse()
            {
                registration = voucherGenerationRequest.registration,
                productSku = voucherGenerationRequest.productSku,
                voucherCode = voucherCode,
                voucherBalance = tokenBalance //VoucherGenerationService.tokenBalance
            };
        }
    }
}