using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Beis.HelpToGrow.Api.Voucher.Services.HealthCheck
{
    public class DatabaseHealthCheckService : IHealthCheck
    {
        private readonly HtgVendorSmeDbContext dbContext;
        private readonly ILogger<DatabaseHealthCheckService> logger;

        public DatabaseHealthCheckService(HtgVendorSmeDbContext dbContext, ILogger<DatabaseHealthCheckService> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
       HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var isHealthy = true;

            try
            {
                if (!dbContext.Database.CanConnect())
                {
                    isHealthy = false;
                    logger.LogError("Database Healthcheck failed. No database connection");
                }
            }
            catch (Exception e)
            {
                isHealthy = false;
                logger.LogError(e, "Database Healthcheck failed.");
            }

            // ...

            if (isHealthy)
            {
                return Task.FromResult(
                    HealthCheckResult.Healthy("Database Healthcheck passed."));
            }

            return Task.FromResult(
                new HealthCheckResult(
                    context.Registration.FailureStatus, "Database Healthcheck failed."));
        }
    }
}
