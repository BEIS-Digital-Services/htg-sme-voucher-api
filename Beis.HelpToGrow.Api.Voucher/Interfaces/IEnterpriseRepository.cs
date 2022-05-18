
namespace Beis.HelpToGrow.Api.Voucher.Interfaces
{
    public interface IEnterpriseRepository
    {
        Task<enterprise> GetEnterprise(long enterpriseId);
    }
}