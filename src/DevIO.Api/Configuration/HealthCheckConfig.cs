using DevIO.Api.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DevIO.Api.Configuration
{
    public static class HealthCheckConfig
    {
        public static IServiceCollection AddHealthCheckConfig(this IServiceCollection services, IConfiguration Configuration)
        {
            //adding health check services to container
            services.AddHealthChecks()
                   .AddCheck("Produtos", new SqlServerHealthCheck(Configuration.GetConnectionString("DefaultConnection")))
                   .AddSqlServer(
                                  connectionString: Configuration.GetConnectionString("DefaultConnection"),
                                  healthQuery: "SELECT 1;",
                                  name: "SQL Server",
                                  failureStatus: HealthStatus.Degraded,
                                  tags: new string[] { "db", "sql", "sqlserver" });

            //adding healthchecks UI
            services.AddHealthChecksUI(opt =>
            {
                opt.SetEvaluationTimeInSeconds(15); //time in seconds between check
                opt.MaximumHistoryEntriesPerEndpoint(60); //maximum history of checks
                opt.SetApiMaxActiveRequests(1); //api requests concurrency
            })
             .AddInMemoryStorage();

            return services;
        }

        public static IApplicationBuilder UseHealthCheckConfig(this IApplicationBuilder app)
        {
            app.UseRouting()
              .UseEndpoints(config =>
              {
                  config.MapHealthChecks("/healthchecks", new HealthCheckOptions
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
}