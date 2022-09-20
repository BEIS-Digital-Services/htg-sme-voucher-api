using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Beis.HelpToGrow.Api.Voucher.Extensions
{
    internal static class RegisterVoucherApiServicesExtension
    {
        public static IServiceCollection RegisterVoucherApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<HealthcheckPublisherSettings>().Bind(configuration).ValidateDataAnnotations();
            services.AddOptions<EncryptionSettings>().Bind(configuration).ValidateDataAnnotations();
            services.AddOptions<VoucherSettings>().Bind(configuration.GetSection("VoucherSettings")).ValidateDataAnnotations();            
            services.AddLogging(options =>
            {
                // hook the Console Log Provider
                options.AddConsole();
                options.SetMinimumLevel(LogLevel.Trace);

            });
            services.AddScoped<IApplicationInsightsPublisher, ApplicationInsightsPublisher>();
            services.AddApplicationInsightsTelemetry(configuration["AzureMonitorInstrumentationKey"]);

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
            services.AddTransient<IVoucherRedeemService, VoucherRedeemService>();
            services.AddTransient<IVoucherCancellationService, VoucherCancellationService>();
            services.AddTransient<IProductPriceRepository, ProductPriceRepository>();
            services.AddVoucherPersistence(configuration);

            services.AddHealthChecks()
                //.AddDbContextCheck<HtgVendorSmeDbContext>("Database Connection", HealthStatus.Unhealthy, new[] { HealthCheckType.Database.ToString() })
                .AddCheck<DatabaseHealthCheckService>("Database Connection", HealthStatus.Unhealthy, new[] { HealthCheckType.Database.ToString() })
                .AddCheck<DependencyInjectionHealthCheckService>("Dependency Injection", HealthStatus.Unhealthy, new[] {HealthCheckType.DI.ToString()})
                .AddCheck<Common.Voucher.Services.HealthChecks.EncryptionHealthCheckService>("Encryption", failureStatus: HealthStatus.Unhealthy, new[] { HealthCheckType.Encryption.ToString() });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "beis-htg-sme-voucher-api-service", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });


            bool.TryParse(configuration["ShowGenerateTestVoucherEndPoint"], out var showGenerateTestVoucherEndPoint);
            services.AddTransient(o => new CanGenerateTestVoucherAttribute(showGenerateTestVoucherEndPoint));

            return services;
        }       
    }
}
