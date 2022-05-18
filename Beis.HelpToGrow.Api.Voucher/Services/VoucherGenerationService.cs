
namespace Beis.HelpToGrow.Api.Voucher.Services
{
    public class VoucherGenerationService: IVoucherGeneratorService
    {
        private int months => 3;
        public static decimal tokenBalance => 5000M;
        private readonly IEncryptionService _encryptionService;
        private int voucherCodeLength => 10;
        private ITokenRepository _tokenRepository;

        public VoucherGenerationService(IEncryptionService encryptionService, ITokenRepository tokenRepository)
        {
            _encryptionService = encryptionService;
            _tokenRepository = tokenRepository;
        }
        
        public async Task<string> GenerateVoucher(vendor_company vendorCompany, enterprise enterprise, product product)
        { 
            var voucherCodeString = GenerateSetCode(voucherCodeLength);
            string encryptedToken = cleanToken(_encryptionService.Encrypt(voucherCodeString, vendorCompany.registration_id + vendorCompany.vendorid));
            token token = new token
            {
                token_code = voucherCodeString,
                token_balance = tokenBalance,
                enterprise_id = enterprise.enterprise_id,
                token_valid_from = DateTime.Now,
                token_expiry =  DateTime.Now.AddMonths(months),
                redemption_status_id = 0,
                product = product.product_id,
                reconciliation_status_id = 0,
                authorisation_code = GenerateSetCode(voucherCodeLength),
                obfuscated_token = $"**********{encryptedToken.Substring(encryptedToken.Length - 4)}"
            };
            
            await _tokenRepository.AddToken(token);

            return encryptedToken;
        }

        private static string cleanToken(string encryptedToken)
        {
            return encryptedToken.EndsWith("==")
                    ? encryptedToken[..^2]
                    : encryptedToken;
        }
        public string GenerateSetCode(int length)
        {
            var rangePaddingFormat = "0";
            foreach (int value in Enumerable.Range(1, length - 1))
            {
                rangePaddingFormat += "0";
            }

            int startNumber = 10;
            int maxLength = 10;
            for (int i = 1; i < length; i++)
            {
                maxLength = maxLength * startNumber;
            }
            return new Random().Next(0, maxLength).ToString(rangePaddingFormat);
        }
    }
}