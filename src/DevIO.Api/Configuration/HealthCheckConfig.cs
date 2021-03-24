using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevIO.Api.Configuration
{
    public static class HealthCheckConfig
    {
        public static IServiceCollection AddHealthCheckConfig(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddHealthChecks()
                     .AddSqlServer(Configuration.GetConnectionString("DefaultConnection"), name: "sqlserver");

            services.AddHealthChecksUI().AddInMemoryStorage();

            return services;
        }

        public static IApplicationBuilder UseHealthCheckConfig(this IApplicationBuilder app)
        {
            // Gera o endpoint que retornará os dados utilizados no dashboard
            app.UseHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            // Ativa o dashboard para a visualização da situação de cada Health Check
            app.UseHealthChecksUI(options =>
            {
                options.UIPath = "/monitor";
            });

            return app;
        }

    }
}