
namespace Beis.HelpToGrow.Api.Voucher.Interfaces
{
    public interface IProductRepository
    {
        Task<product> GetProductSingle(long id);
        product GetProductBySku(string productSku, long vendorId);
    }
}