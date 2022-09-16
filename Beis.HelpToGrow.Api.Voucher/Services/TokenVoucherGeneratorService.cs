
using Beis.HelpToGrow.Common.Voucher.Config;
using Beis.HelpToGrow.Common.Voucher.Interfaces;

namespace Beis.HelpToGrow.Api.Voucher.Services
{
    public class TokenVoucherGeneratorService : ITokenVoucherGeneratorService
    {
        private readonly IProductRepository _productRepository;
        private readonly IVendorCompanyRepository _vendorCompanyRepository;
        private readonly IVoucherGenerationService _voucherGeneratorService;
        private readonly IOptions<VoucherSettings> _voucherSettings;

        public TokenVoucherGeneratorService(IProductRepository productRepository,
            IVendorCompanyRepository vendorCompanyRepository,
            IVoucherGenerationService voucherGeneratorService,
            IOptions<VoucherSettings> voucherSettings)
        {
            _productRepository = productRepository;
            _vendorCompanyRepository = vendorCompanyRepository;
            _voucherGeneratorService = voucherGeneratorService;
            _voucherSettings = voucherSettings;
        }
        private static decimal tokenBalance => 5000M;
        public async Task<VoucherGenerationResponse> GenerateVoucher(VoucherGenerationRequest voucherGenerationRequest)
        {

            var vendorCompany = _vendorCompanyRepository.GetVendorCompanyByRegistration(voucherGenerationRequest.Registration);
            if (vendorCompany == null)
            {
                return new VoucherGenerationResponse()
                {
                    Status = "ERROR",
                    Message = "Company registration not found."
                };
            }
            var product = _productRepository.GetProductBySku(voucherGenerationRequest.ProductSku, vendorCompany.vendorid);

            if (product == null)
            {
                return new VoucherGenerationResponse()
                {
                    Status = "ERROR",
                    Message = "Product not found."
                };
            }
            var enterprise = new enterprise()
            {
                enterprise_id = 1
            };

            var voucherCode = await _voucherGeneratorService.GenerateVoucher(vendorCompany, enterprise, product, _voucherSettings,false);

            return new VoucherGenerationResponse()
            {
                Registration = voucherGenerationRequest.Registration,
                ProductSku = voucherGenerationRequest.ProductSku,
                VoucherCode = voucherCode,
                VoucherBalance = tokenBalance //VoucherGenerationService.tokenBalance
            };
        }
    }
}