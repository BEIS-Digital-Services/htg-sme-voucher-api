using Beis.HelpToGrow.Api.Voucher.Services.HealthCheck;
using Beis.HelpToGrow.Common.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Beis.HelpToGrow.Common.Services;

namespace Beis.HelpToGrow.Api.Voucher.Extensions
{
    public static class RegisterVoucherApiServicesExtension
    {
        public static IServiceCollection RegisterVoucherApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EncryptionSettings>(options =>
                configuration.Bind(options));

            services.AddLogging(options =>
            {
                // hook the Console Log Provider
                options.AddConsole();
                options.SetMinimumLevel(LogLevel.Trace);

            });
            services.AddApplicationInsightsTelemetry(configuration["AZURE_MONITOR_INSTRUMENTATION_KEY"]);

            services.AddSingleton<IEncryptionService, AesEncryption>();
            

            services.AddTransient<IVoucherCheckService, VoucherCheckService>();
            services.AddTransient<IVendorAPICallStatusServices, VendorAPICallStatusServices>();

            services.AddTransient<ITokenRepository, TokenRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IVendorCompanyRepository, VendorCompanyRepository>();
            services.AddTransient<IVendorAPICallStatusRepository, VendorApiCallStatusRepository>();
            services.AddTransient<IEnterpriseRepository, EnterpriseRepository>();
            services.AddTransient<ITokenVoucherGeneratorService, TokenVoucherGeneratorService>();
            services.AddTransient<IVoucherGenerationService, VoucherGenerationService>();    
            services.AddTransient<IVendorReconciliationSalesRepository, VendorReconciliationSalesRepository>();
            services.AddTransient<IVendorReconciliationRepository, VendorReconciliationRepository>();         
            services.AddTransient<IVoucherReconciliationService, VoucherReconciliationService>();
            //services.AddDbContext<HtgVendorSmeDbContext>(options => options.UseNpgsql(configuration["HELPTOGROW_CONNECTIONSTRING"]), ServiceLifetime.Transient);
            services.AddTransient<IVoucherRedeemService, VoucherRedeemService>();
            services.AddTransient<IVoucherCancellationService, VoucherCancellationService>();
            services.AddTransient<IProductPriceRepository, ProductPriceRepository>();
            services.AddVoucherPersistence(configuration);



            services.AddHealthChecks()
                //.AddCheck<StartupHealthCheckService>(
                //    "Startup",
                //    tags: new[] { "ready" })
                .AddDbContextCheck<HtgVendorSmeDbContext>()
                .AddCheck<DatabaseHealthCheckService>("Database")
                .AddCheck<EncryptionHealthCheckService>("Encryption", failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "Encryption" });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "beis-htg-sme-voucher-api-service", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            return services;
        }       
    }
}
