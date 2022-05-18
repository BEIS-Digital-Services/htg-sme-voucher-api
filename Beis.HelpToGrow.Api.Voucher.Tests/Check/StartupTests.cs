//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc.Testing;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using Moq;
//using NUnit.Framework;
//using Swashbuckle.AspNetCore.Swagger;
//using Swashbuckle.AspNetCore.SwaggerGen;
//using System;

//namespace Beis.HelpToGrow.Voucher.Api.Check.Tests
//{
//    [TestFixture]
//    public class StartupTests
//    {
//        private WebApplicationFactory<Program> _factory; // todo come back to this : https://docs.microsoft.com/en-us/aspnet/core/migration/50-to-60-samples?view=aspnetcore-6.0

//        [SetUp]
//        public void Setup()
//        {
//            Environment.SetEnvironmentVariable("VOUCHER_ENCRYPTION_SALT", "UnitTestSalt");
//            Environment.SetEnvironmentVariable("VOUCHER_ENCRYPTION_ITERATION", "1");
//            Environment.SetEnvironmentVariable("VOUCHER_ENCRYPTION_INITIAL_VECTOR", "UnitTestVector");
//            Environment.SetEnvironmentVariable("VOUCHER_ENCRYPTION_KEY_SIZE", "128");
//            Environment.SetEnvironmentVariable("HELPTOGROW_CONNECTIONSTRING", "dbConnectionStr");
//            _factory = new WebApplicationFactory<Program>();
//        }

//        [Test]
//        public void CheckStartup()
//        {
//            using (var scope = _factory.Services.CreateScope())
//            {
//                var serviceProvider = scope.ServiceProvider;
//                Assert.IsNotNull(serviceProvider.GetService(typeof(ISwaggerProvider)));

//                Assert.IsNotNull(serviceProvider.GetService(typeof(IWebHostEnvironment)));
//                Assert.IsNotNull(serviceProvider.GetService(typeof(IEncryptionService)));
//                Assert.IsNotNull(serviceProvider.GetService(typeof(IVoucherCheckService)));
//                Assert.IsNotNull(serviceProvider.GetService(typeof(ITokenVoucherGeneratorService)));
//                Assert.IsNotNull(serviceProvider.GetService(typeof(IVoucherGeneratorService)));
//                Assert.IsNotNull(serviceProvider.GetService(typeof(ITokenRepository)));
//                Assert.IsNotNull(serviceProvider.GetService(typeof(IProductRepository)));
//                Assert.IsNotNull(serviceProvider.GetService(typeof(IVendorCompanyRepository)));
//                Assert.IsNotNull(serviceProvider.GetService(typeof(IEnterpriseRepository)));
//            }

//        }

//        [TearDown]
//        public void TearDown()
//        {
//            _factory?.Dispose();
//        }
//    }
//}

