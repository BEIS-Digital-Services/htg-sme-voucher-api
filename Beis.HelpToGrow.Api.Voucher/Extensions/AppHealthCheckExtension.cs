using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Beis.HelpToGrow.Api.Voucher.Extensions
{
    public static class AppHealthCheckExtension
    {
        public static IEndpointConventionBuilder MapApiHealthChecks(this IEndpointRouteBuilder builder )
        {
            var result = builder.MapHealthChecks("/healthz", new HealthCheckOptions
            {
                Predicate = (check) => false,
                ResponseWriter = HealthCheckJsonResponseWriter.Write
            });
            result = builder.MapHealthChecks("/healthz/all", new HealthCheckOptions
            {
                ResponseWriter = HealthCheckJsonResponseWriter.Write
            });
            result = builder.MapHealthChecks("/healthz/di", new HealthCheckOptions
            {
                Predicate = (check) => check.Tags.Contains(HealthCheckType.DI.ToString()),
                ResponseWriter = HealthCheckJsonResponseWriter.Write
            });
            result = builder.MapHealthChecks("/healthz/encryption", new HealthCheckOptions
            {
                Predicate = (check) => check.Tags.Contains(HealthCheckType.Encryption.ToString()),
                ResponseWriter = HealthCheckJsonResponseWriter.Write
            });
            result = builder.MapHealthChecks("/healthz/database", new HealthCheckOptions
            {
                Predicate = (check) => check.Tags.Contains(HealthCheckType.Database.ToString()),
                ResponseWriter = HealthCheckJsonResponseWriter.Write
            });
            return result;            
        }        
    }    
}
