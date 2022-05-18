

namespace Beis.HelpToGrow.Api.Voucher.Interfaces
{
    public interface IVendorReconciliationSalesRepository
    {
        Task AddVendorReconciliationSales(vendor_reconciliation_sale vendorReconciliationSales, token token);
        Task<vendor_reconciliation_sale> GetVendorReconciliationSalesByVoucherCode(string voucherCode);
    }
}