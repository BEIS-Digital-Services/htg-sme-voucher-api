
namespace Beis.HelpToGrow.Api.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly HtgVendorSmeDbContext _context;

        public ProductRepository(HtgVendorSmeDbContext context)
        {
            _context = context;
        }


        public async Task<product> GetProductSingle(long id)
        {
            return await _context.products.FirstOrDefaultAsync(t => t.product_id == id);
        }

        public product GetProductBySku(string productSku, long vendorId)
        {
            var product = _context.products.Include("product_prices")
                .Where(t => t.product_SKU == productSku && t.vendor_id == vendorId)
                .Select(p => new product()
                {
                    product_id = p.product_id
                }).Single();
            return product;
        }
    }
}
