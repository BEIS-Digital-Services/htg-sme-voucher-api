using Microsoft.Extensions.Diagnostics.HealthChecks;


//https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0#customize-output
namespace Beis.HelpToGrow.Api.Voucher.Services.HealthCheck
{
    public class StartupHealthCheckService : IHealthCheck
    {
        private volatile bool _isReady;

        public bool StartupCompleted
        {
            get => _isReady;
            set => _isReady = value;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (StartupCompleted)
            {
                return Task.FromResult(HealthCheckResult.Healthy("The startup task has completed."));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy("That startup task is still running."));
        }
    }
}
