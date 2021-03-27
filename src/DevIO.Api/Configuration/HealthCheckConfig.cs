using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
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
                    .AddMongoDb(mongodbConnectionString: Configuration.GetConnectionString("DefaultMongo"),
                            name: "mongo",
                            failureStatus: HealthStatus.Unhealthy); //adding MongoDb Health Check

            //adding healthchecks UI
            services.AddHealthChecksUI(opt =>
            {
                opt.SetEvaluationTimeInSeconds(15); //time in seconds between check
                opt.MaximumHistoryEntriesPerEndpoint(60); //maximum history of checks
                opt.SetApiMaxActiveRequests(1); //api requests concurrency

                opt.AddHealthCheckEndpoint("default api", "/healthz"); //map health check api
            })
            .AddInMemoryStorage();

            return services;
        }

        public static IApplicationBuilder UseHealthCheckConfig(this IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                //adding endpoint of health check for the health check ui in UI format
                endpoints.MapHealthChecks("/healthz", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

                //map healthcheck ui endpoing - default is /healthchecks-ui/
                endpoints.MapHealthChecksUI();

                endpoints.MapGet("/", async context => await context.Response.WriteAsync("Hello World!"));
            });

            return app;
        }

    }
}