using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

namespace DevIO.Api.Configuration
{
    public static class HealthCheckConfig
    {
        public static IServiceCollection AddHealthCheckConfig(this IServiceCollection services, IConfiguration Configuration)
        {
            //adding health check services to container
            services.AddHealthChecks()
                   .AddCheck<ExampleHealthCheck>("example_health_check");

            //adding healthchecks UI
            services.AddHealthChecksUI(opt =>
            {
                opt.SetEvaluationTimeInSeconds(15); //time in seconds between check
                opt.MaximumHistoryEntriesPerEndpoint(60); //maximum history of checks
                opt.SetApiMaxActiveRequests(1); //api requests concurrency

                opt.AddHealthCheckEndpoint("default api", "/healthz"); //map health check api
            })
             .AddSqlServerStorage(Configuration.GetConnectionString("DefaultConnection"));

            return services;
        }

        public static IApplicationBuilder UseHealthCheckConfig(this IApplicationBuilder app)
        {
            app.UseRouting()
              .UseEndpoints(config =>
              {
                  config.MapHealthChecks("/healthz", new HealthCheckOptions
                  {
                      Predicate = _ => true,
                      ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                  });

                  config.MapHealthChecksUI(setup =>
                  {
                      setup.UIPath = "/show-health-ui"; // this is ui path in your browser
                      setup.ApiPath = "/health-ui-api"; // the UI ( spa app )  use this path to get information from the store ( this is NOT the healthz path, is internal ui api )
                  });

                  config.MapDefaultControllerRoute();
              });
            return app;
        }
    }

    public class ExampleHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var healthCheckResultHealthy = true;

            if (healthCheckResultHealthy)
            {
                return Task.FromResult(
                    HealthCheckResult.Healthy("A healthy result."));
            }

            return Task.FromResult(
                HealthCheckResult.Unhealthy("An unhealthy result."));
        }
    }
}