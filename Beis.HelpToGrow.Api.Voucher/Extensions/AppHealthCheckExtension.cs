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
                ResponseWriter = WriteHealthResponse
            });


            //result.MapHealthChecks("/healthz/ready", new HealthCheckOptions
            //{
            //    Predicate = healthCheck => healthCheck.Tags.Contains("ready")
            //});

            //result.MapHealthChecks("/healthz/live", new HealthCheckOptions
            //{
            //    Predicate = _ => false
            //});
            return result;

            
        }

        internal static Task WriteHealthResponse(HttpContext context, HealthReport healthReport)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            var options = new JsonWriterOptions { Indented = true };

            using var memoryStream = new MemoryStream();
            using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
            {
                jsonWriter.WriteStartObject();
                jsonWriter.WriteString("status", healthReport.Status.ToString());
                jsonWriter.WriteStartObject("results");

                foreach (var healthReportEntry in healthReport.Entries)
                {
                    jsonWriter.WriteStartObject(healthReportEntry.Key);
                    jsonWriter.WriteString("status",
                        healthReportEntry.Value.Status.ToString());
                    jsonWriter.WriteString("description",
                        healthReportEntry.Value.Description);
                    jsonWriter.WriteStartObject("data");

                    foreach (var item in healthReportEntry.Value.Data)
                    {
                        jsonWriter.WritePropertyName(item.Key);

                        JsonSerializer.Serialize(jsonWriter, item.Value,
                            item.Value?.GetType() ?? typeof(object));
                    }

                    jsonWriter.WriteEndObject();
                    jsonWriter.WriteEndObject();
                }

                jsonWriter.WriteEndObject();
                jsonWriter.WriteEndObject();
            }

            return context.Response.WriteAsync(
                Encoding.UTF8.GetString(memoryStream.ToArray()));
        }
    }


    
}
