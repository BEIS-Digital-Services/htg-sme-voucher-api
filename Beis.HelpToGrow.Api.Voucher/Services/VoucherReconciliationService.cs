
namespace Beis.HelpToGrow.Api.Voucher.Services
{
    public class VoucherReconciliationService: IVoucherReconciliationService
    {
        private const int reconciled = 2;
        private const int redeemed = 2;
        private const int notRedeemed = 0;
        private readonly IEncryptionService _encryptionService;
        private readonly ITokenRepository _tokenRepository;
        private readonly IProductRepository _productRepository;
        private readonly IVendorCompanyRepository _vendorCompanyRepository;
        private readonly IVendorReconciliationSalesRepository _reconciliationSalesRepository;
        private readonly IVendorReconciliationRepository _vendorReconciliationRepository;
        private ILogger<VoucherReconciliationService> _logger;
        private IVendorAPICallStatusServices _vendorApiCallStatusService;

        public VoucherReconciliationService(ILogger<VoucherReconciliationService> logger, IEncryptionService encryptionService, ITokenRepository tokenRepository,
            IProductRepository productRepository,
            IVendorCompanyRepository vendorCompanyRepository,
            IVendorReconciliationRepository vendorReconciliationRepository,
            IVendorReconciliationSalesRepository vendorReconciliationSalesRepository, 
            
            IVendorAPICallStatusServices vendorApiCallStatusServices)
        {
            _encryptionService = encryptionService;
            _tokenRepository = tokenRepository;
            _productRepository = productRepository;
            _vendorCompanyRepository = vendorCompanyRepository;
            _vendorReconciliationRepository = vendorReconciliationRepository;
            _reconciliationSalesRepository = vendorReconciliationSalesRepository;
            _logger = logger;
            _vendorApiCallStatusService = vendorApiCallStatusServices;
        }

        private void logAPiCallStatus(vendor_api_call_status vendor_api_call_status, VoucherReconciliationResponse voucherResponse)
        {
            vendor_api_call_status.result = JsonSerializer.Serialize(voucherResponse);
            _vendorApiCallStatusService.LogRequestDetails(vendor_api_call_status);
        }

        public async Task<VoucherReconciliationResponse> GetVoucherResponse(VoucherReconciliationRequest voucherReconciliationRequest)
        {
            _logger.LogInformation("VoucherReconciliationServiceRequest: {@VoucherReconciliationRequest}",JsonSerializer.Serialize(voucherReconciliationRequest));
            
            VoucherReconciliationResponse voucherResponse = new VoucherReconciliationResponse();
        
            List<VoucherReconciliationReport> reciliationReport = new List<VoucherReconciliationReport>();
            try
            {
                var vendorCompanySingle = _vendorCompanyRepository.GetVendorCompanyByRegistration(voucherReconciliationRequest.Registration);
                if(vendorCompanySingle != null && IsValidVendor(voucherReconciliationRequest, vendorCompanySingle))
                {
                    foreach (var dailySalesSale in voucherReconciliationRequest.DailySales.Sales)
                    {
                        var voucherReport = new VoucherReconciliationReport();
                        try
                        {
                            var decryptedVoucherCode = DecryptVoucher(dailySalesSale.VoucherCode, vendorCompanySingle);
                            var token = GetToken(decryptedVoucherCode);
                            
                            if(token == null)
                            {
                                voucherResponse.Status = "ERROR";
                                voucherResponse.ErrorCode = 20;

                                voucherReport.Status = "ERROR";
                                voucherReport.VoucherCode = dailySalesSale.VoucherCode;
                                voucherReport.Reason = "Unknown token";
                                reciliationReport.Add(voucherReport);
                                continue;
                            }

                            if (token.cancellation_status_id.HasValue)
                            {
                                voucherResponse.Status = "ERROR";
                                voucherResponse.ErrorCode = 30;

                                voucherReport.Status = "ERROR";
                                voucherReport.VoucherCode = dailySalesSale.VoucherCode;
                                voucherReport.Reason = "Cancelled token";
                                reciliationReport.Add(voucherReport);
                                continue;
                            }
                            var productId = token.product;
                            var product = await _productRepository.GetProductSingle(productId);

                            await ProcessDailySale(token, dailySalesSale, decryptedVoucherCode, vendorCompanySingle, product, reciliationReport);
                            
                            voucherReport.Status = "Success";
                            voucherReport.VoucherCode = dailySalesSale.VoucherCode;
                            
                            //reciliationReport.Add(voucherReport);
                        }
                        //catch (VoucherException e)
                        //{
                        //    voucherResponse.status = "ERROR";
                        //    voucherResponse.errorCode = e.errorCode;
                            
                        //    voucherReport.status = "ERROR";
                        //    voucherReport.voucherCode = dailySalesSale.voucherCode;
                        //    voucherReport.reason = e.message;
                        //    reciliationReport.Add(voucherReport);
                        //}
                        catch (Exception e)
                        {
                            voucherResponse = new VoucherReconciliationResponse
                            {
                                Status = "ERROR",
                                ErrorCode = 10,
                                Message = "Error in format"
                            };
                            
                            voucherReport.Status = "ERROR";
                            voucherReport.VoucherCode = dailySalesSale.VoucherCode;
                            voucherReport.Reason = e.Message;
                            reciliationReport.Add(voucherReport);
                        }
                    }                    
                }
                else
                {
                    //throw new Exception("Invalid vendor details");
                    voucherResponse.Status = "ERROR";
                    voucherResponse.ErrorCode = 10;
                    voucherResponse.Message = "Unknown vendor details";
                    _logger.LogError("VoucherReconciliationResponse: {@VoucherResponse}, {@VErrorMessage}", JsonSerializer.Serialize(voucherResponse), "Invalid vendor details");
                }
            }
            catch (Exception e)
            {
                voucherResponse.Status = "ERROR";
                voucherResponse.ErrorCode = 10;
                voucherResponse.Message = "Unknown vendor details";
                _logger.LogError("VoucherReconciliationResponse: {@VoucherResponse}, {@VErrorMessage}",JsonSerializer.Serialize(voucherResponse), e.Message);
            }
            
            voucherResponse.ReconciliationReport = reciliationReport;
            if(reciliationReport.Any(x => x.Status == "ERROR"))
            {
                
                voucherResponse.Status = "ERROR";
                if (voucherResponse.ErrorCode == 0)
                    voucherResponse.ErrorCode = 10;
            }
            _logger.LogInformation("VoucherReconciliationResponse: {@VoucherResponse}", JsonSerializer.Serialize(voucherResponse));
            
            var vendor_api_call_status = _vendorApiCallStatusService.CreateLogRequestDetails(voucherReconciliationRequest);
            vendor_api_call_status.error_code = voucherResponse.ErrorCode == 0 ? "200" : "400";
            logAPiCallStatus(vendor_api_call_status, voucherResponse);
            return voucherResponse;
        }
        
        private async Task ProcessDailySale(token token, SalesReconcilliation reconciliation, string tokenCode, vendor_company vendorCompanySingle, product product, List<VoucherReconciliationReport> reciliationReport)
        {
            
            var tokenBalance = token.token_balance - reconciliation.DiscountApplied;

            if (tokenBalance < 0)
            {
                reciliationReport.Add(new VoucherReconciliationReport 
                { 
                    Status = "ERROR",
                    Reason = "Reconciliation discount applied " + reconciliation.DiscountApplied + " more than voucher balance " + token.token_balance,
                    VoucherCode = reconciliation.VoucherCode

                });
                return;
            }

            token.token_balance = tokenBalance;

            var reconciliationSales = await _reconciliationSalesRepository.GetVendorReconciliationSalesByVoucherCode(tokenCode);
            if (reconciliationSales != null)
            {
                reciliationReport.Add(new VoucherReconciliationReport
                {
                    Status = "ERROR",
                    Reason = "Already reconciled",
                    VoucherCode = reconciliation.VoucherCode

                });
                return;
            }
            
            if (product.product_SKU.Equals(reconciliation.ProductSku))
            {
                if (reconciliation.AuthorisationCode.Equals(token.authorisation_code))
                {
                    if (token.redemption_status_id == notRedeemed)
                    {
                        reciliationReport.Add(new VoucherReconciliationReport
                        {
                            Status = "ERROR",
                            Reason = "Please redeem voucher before proceeding",
                            VoucherCode = reconciliation.VoucherCode

                        });
                        return;
                    }
                    
                    if (token.redemption_status_id is redeemed)
                    {
                        reciliationReport.Add(new VoucherReconciliationReport
                        {
                            Status = "ERROR",
                            Reason = "Already redeemed",
                            VoucherCode = reconciliation.VoucherCode

                        });
                        return;
                    }
                    
                    if (token.reconciliation_status_id is reconciled)
                    {
                        reciliationReport.Add(new VoucherReconciliationReport
                        {
                            Status = "ERROR",
                            Reason = "Already reconciled",
                            VoucherCode = reconciliation.VoucherCode

                        });
                        return;
                    }

                    vendor_reconciliation vendorReconciliation = new vendor_reconciliation()
                    {
                        reconciliation_id = DateTime.Now.Ticks,
                        vendor_id = vendorCompanySingle.vendorid,
                        reconciliation_date = DateTime.Now
                    };
                    
                    var vendorReconciliationSales = new vendor_reconciliation_sale()
                    {
                        reconciliation_sales_id = vendorReconciliation.reconciliation_id,
                        token_code = tokenCode,
                        vendor_id = vendorCompanySingle.registration_id,
                        product_sku = product.product_SKU,
                        product_name = product.product_name,
                        licensed_to = reconciliation.LicenceTo,
                        sme_email = reconciliation.SmeEmail,
                        purchaser_name = reconciliation.PurchaserName,
                        one_off_cost = reconciliation.OneOffCosts,
                        no_of_licenses = reconciliation.NoOfLicences,
                        cost_per_license = reconciliation.CostPerLicence,
                        total_amount = reconciliation.TotalAmount,
                        discount_applied = reconciliation.DiscountApplied,
                        currency = reconciliation.Currency,
                        contract_term_months = reconciliation.ContractTermInMonths,
                        trial_period_months = reconciliation.TrialPeriodInMonths
                        
                    };
                   
                   vendorReconciliationSales.reconciliation_sales_id = vendorReconciliation.reconciliation_id;
                   vendorReconciliation.reconciliation = vendorReconciliationSales;
                   _vendorReconciliationRepository.AddVendorReconciliation(vendorReconciliation);

                   token.reconciliation_status_id = reconciled;
                   token.redemption_status_id = redeemed;

                   await _tokenRepository.UpdateToken(token);
                   
                }
                else
                {
                    reciliationReport.Add(new VoucherReconciliationReport
                    {
                        Status = "ERROR",
                        Reason = "Invalid authorisationCode",
                        VoucherCode = reconciliation.VoucherCode

                    });
                    return;
                }

                reciliationReport.Add(new VoucherReconciliationReport
                {
                    VoucherCode = reconciliation.VoucherCode,
                    Status = "Success",                    
                });
            }
            else
            {
                reciliationReport.Add(new VoucherReconciliationReport
                {
                    Status = "ERROR",
                    Reason = "Invalid product_SKU",
                    VoucherCode = reconciliation.VoucherCode

                });
                return;
            }

        }
        
        private string DecryptVoucher(string encryptedVoucherCode, vendor_company vendorCompany)
        {
            var voucherCode = encryptedVoucherCode + "==";
            return _encryptionService.Decrypt(voucherCode, vendorCompany.registration_id + vendorCompany.vendorid);
        }
        
        public token GetToken(string decryptedVoucherCode)
        {
            var token = _tokenRepository.GetTokenByTokenCode(decryptedVoucherCode);

            return token;
        }
        
        public bool IsValidVendor(VoucherReconciliationRequest voucherRequest, vendor_company vendorCompany)
        {
            return voucherRequest.Registration == vendorCompany.registration_id &&
                   voucherRequest.AccessCode == vendorCompany.access_secret;
        }        
    }
}