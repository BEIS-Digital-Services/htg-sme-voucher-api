
namespace Beis.HelpToGrow.Api.Voucher.Services
{
    public class VoucherCheckService : IVoucherCheckService
    {
        const string tokenExpiryFormat = "yyyy-MM-dd'T'HH:mm:ss";
        private readonly IEncryptionService _encryptionService;
        private readonly ITokenRepository _tokenRepository;
        private readonly IProductRepository _productRepository;
        private readonly IVendorCompanyRepository _vendorCompanyRepository;
        private readonly IEnterpriseRepository _enterpriseRepository;
        private ILogger<VoucherCheckService> _logger;
        private IVendorAPICallStatusServices _vendorApiCallStatusService;

        public VoucherCheckService(ILogger<VoucherCheckService> logger, IEncryptionService encryptionService, ITokenRepository tokenRepository, IProductRepository productRepository, IVendorCompanyRepository vendorCompanyRepository, IEnterpriseRepository enterpriseRepository, IVendorAPICallStatusServices vendorApiCallStatusService)
        {
            _encryptionService = encryptionService;
            _tokenRepository = tokenRepository;
            _productRepository = productRepository;
            _vendorCompanyRepository = vendorCompanyRepository;
            _enterpriseRepository = enterpriseRepository;
            _logger = logger;
            _vendorApiCallStatusService = vendorApiCallStatusService;
        }

        private async Task logAPiCallStatus(vendor_api_call_status vendor_api_call_status, VoucherCheckResponse VoucherResponse)
        {
            vendor_api_call_status.result = JsonSerializer.Serialize(VoucherResponse);
            await _vendorApiCallStatusService.LogRequestDetails(vendor_api_call_status);
        }

        private async Task<VoucherCheckResponse> getVoucherErrorResponse(VoucherCheckRequest voucherRequest, int errorCode, string message)
        {
            _logger.LogError("There was an error checking the voucher ({errorCode}) : {Message}", errorCode, message);
            var vendorApiCallStatus = _vendorApiCallStatusService.CreateLogRequestDetails(voucherRequest);

            var VoucherResponse = new VoucherCheckResponse
            {
                Status = "ERROR",
                ErrorCode = errorCode,
                Message = message,
                VoucherCode = voucherRequest.VoucherCode
            };
            

            vendorApiCallStatus.error_code = "400";
            await logAPiCallStatus(vendorApiCallStatus, VoucherResponse);
            return VoucherResponse;
        }
        public async Task<VoucherCheckResponse> GetVoucherResponse(VoucherCheckRequest voucherRequest)
        {
            _logger.LogInformation("VoucherCheckServiceRequest: {@VoucherRequest}", JsonSerializer.Serialize(voucherRequest));
         
            try
            {
                _logger.LogInformation("Getting vendor company for registration {registration}", voucherRequest.Registration);
                var vendorCompany = _vendorCompanyRepository.GetVendorCompanyByRegistration(voucherRequest.Registration);
                if (vendorCompany != null)
                {
                    _logger.LogInformation("Decrypting voucher");
                    var decryptedVoucherCode = DecryptVoucher(voucherRequest, vendorCompany);
                    _logger.LogInformation("Getting token for decrypted voucher code {decryptedVoucherCode}", decryptedVoucherCode);
                    var token = GetToken(decryptedVoucherCode);
                    _logger.LogInformation("Got Token for decrypted voucher code {code}", decryptedVoucherCode);
                    if (token != null)
                    {
                        // reject tokens with any cancellation status
                        if (token.cancellation_status_id.HasValue)
                        {
                            return await getVoucherErrorResponse(voucherRequest, 50, "Cancelled token");
                        }
                        var productId = token.product;
                        _logger.LogInformation("Getting product for product Id {productId}", productId);
                        var product = await _productRepository.GetProductSingle(productId);

                        if (IsValidVendor(voucherRequest, vendorCompany))
                        {
                            return await GetVoucherCheckResponse(token, voucherRequest, vendorCompany, product);
                        }
                        return await getVoucherErrorResponse(voucherRequest, 10, "Unknown token Invalid vendor details");
                    }
                    
                    return await getVoucherErrorResponse(voucherRequest, 10, "Unknown token Unknown token");
                }
                return await getVoucherErrorResponse(voucherRequest, 10, "Unknown token Invalid Vendor company");
                
            }

            catch (Exception e)
            {                
                _logger.LogError(e, "There was an unxpected error checking the voucher : {Message}", e.Message);
                return await getVoucherErrorResponse(voucherRequest, 10, e.Message);
                
            }
        }

        private token GetToken(string decryptedVoucherCode)
        {
            var token = _tokenRepository.GetTokenByTokenCode(decryptedVoucherCode);

            return token;
        }

        private async Task<VoucherCheckResponse> GetVoucherCheckResponse(token token, VoucherCheckRequest voucherRequest, vendor_company vendorCompanySingle, product product)
        {
            //Check balance > 0
            //check token status
            //check token expiry
            
            _logger.LogInformation("Getting voucher response");
            if (token.token_expiry.CompareTo(DateTime.Now) < 0)
            {
                return await getVoucherErrorResponse(voucherRequest, 20, "Expired Token");
            }

            if (token.token_balance == 0)
            {
                return await getVoucherErrorResponse(voucherRequest, 30, "No Balance");
            }

            if (token.authorisation_code == null)
            {
                return await getVoucherErrorResponse(voucherRequest, 40, "Locked");
            }

            var voucherResponse = new VoucherCheckResponse();
            voucherResponse.Status = "OK";
            voucherResponse.Message = "Successful check - proceed with Voucher";
            voucherResponse.ErrorCode = 0;
            voucherResponse.VoucherCode = voucherRequest.VoucherCode;
            voucherResponse.AuthorisationCode = token.authorisation_code;

            voucherResponse.Vendor = vendorCompanySingle.vendor_company_name;
            voucherResponse.ProductSku = product.product_SKU;
            voucherResponse.ProductName = product.product_name;
            _logger.LogInformation("Getting enterprise for id {id}", token.enterprise_id);
            var enterprise = await _enterpriseRepository.GetEnterprise(token.enterprise_id);

            voucherResponse.LicenceTo = enterprise.enterprise_name;
            voucherResponse.SmeEmail = enterprise.applicant_email_address;
            voucherResponse.PurchaserName = enterprise.applicant_name;

            voucherResponse.MaxDiscountAllowed = token.token_balance;
            voucherResponse.Currency = "GBP";
            voucherResponse.TokenExpiry = token.token_expiry.ToString(tokenExpiryFormat);


            var vendorApiCallStatus = _vendorApiCallStatusService.CreateLogRequestDetails(voucherRequest);
            vendorApiCallStatus.error_code = "200";
            await logAPiCallStatus(vendorApiCallStatus, voucherResponse);

            return voucherResponse;
        }

        private string DecryptVoucher(VoucherCheckRequest voucherRequest, vendor_company vendorCompany)
        {
            var voucherCode = voucherRequest.VoucherCode + "==";
            return _encryptionService.Decrypt(voucherCode, vendorCompany.registration_id + vendorCompany.vendorid);
        }

        private bool IsValidVendor(VoucherCheckRequest voucherRequest, vendor_company vendorCompany)
        {
            return voucherRequest.Registration == vendorCompany.registration_id &&
                voucherRequest.AccessCode == vendorCompany.access_secret;
        }
    }
}

