
namespace Beis.HelpToGrow.Api.Voucher.Interfaces
{
    public interface ITokenRepository
    {
        Task AddToken(token token);
        token GetToken(string tokenCode);
        Task UpdateToken(token token);

    }
}