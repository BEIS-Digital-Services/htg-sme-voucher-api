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
                ResponseWriter = HealthCheckJsonResponseWriter.Write
            });

            return result;            
        }        
    }    
}
