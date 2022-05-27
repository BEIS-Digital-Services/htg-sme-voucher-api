
//namespace Beis.HelpToGrow.Api.Data.Repositories
//{
//    public class VendorCompanyRepository: IVendorCompanyRepository
//    {
//        private readonly HtgVendorSmeDbContext _context;

//        public VendorCompanyRepository(HtgVendorSmeDbContext context)
//        {
//            _context = context;
//        }
//        public vendor_company GetVendorCompanyByRegistration(string registrationId)
//        {

//            var vendorCompany = _context.vendor_companies
//                .Where(t => t.registration_id == registrationId)
//                .Select(p => new vendor_company()
//                {
//                    vendor_company_name = p.vendor_company_name,
//                    vendorid = p.vendorid,
//                    registration_id = p.registration_id,
//                    access_secret = p.access_secret
//                }).Single();
            
//            return vendorCompany;

//        }
//    }
//}