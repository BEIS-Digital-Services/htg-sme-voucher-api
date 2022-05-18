
namespace Beis.HelpToGrow.Api.Data.Repositories
{
    public class TokenRepository: ITokenRepository
    {
        private readonly HtgVendorSmeDbContext _context;

        public TokenRepository(HtgVendorSmeDbContext context)
        {
            _context = context;
        }
        
        public async Task AddToken(token token)
        {
            await _context.tokens.AddAsync(token);
            
            await _context.SaveChangesAsync();
        }

        public token GetToken(string tokenCode)
        {
            var token = _context.tokens.SingleOrDefault(t => t.token_code == tokenCode);        
            return token;
        }
        
        public async Task UpdateToken(token token)
        {
            _context.tokens.Update(token);
            await _context.SaveChangesAsync();
        }
    }
}