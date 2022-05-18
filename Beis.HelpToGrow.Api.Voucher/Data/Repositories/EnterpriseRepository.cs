
namespace Beis.HelpToGrow.Api.Data.Repositories
{
    public class EnterpriseRepository: IEnterpriseRepository
    {
        private readonly HtgVendorSmeDbContext _context;

        public EnterpriseRepository(HtgVendorSmeDbContext context)
        {
            _context = context;
        }
        public async Task<enterprise> GetEnterprise(long enterpriseId)
        {
            return await _context.enterprises.FirstOrDefaultAsync(t => t.enterprise_id == enterpriseId);
        }
    }
}