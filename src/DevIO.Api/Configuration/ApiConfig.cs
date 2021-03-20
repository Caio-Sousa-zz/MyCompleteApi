using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace DevIO.Api.Configuration
{
    public static class ApiConfig
    {
        public static IServiceCollection WebApiConfig(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(op =>
            {
                op.SuppressModelStateInvalidFilter = true;
            });


            services.AddCors(options =>
            {
                options.AddPolicy("Development",
                        builder => builder.AllowAnyOrigin()
                                          .AllowAnyMethod()
                                          .AllowAnyHeader());

                options.AddPolicy("Production",
                   builder => builder.AllowAnyOrigin()
                                     .WithOrigins("https://websiteteste.com")
                                     .SetIsOriginAllowedToAllowWildcardSubdomains()
                                     .AllowAnyMethod()
                                     .AllowAnyHeader()
                                     .AllowCredentials());
            });

            return services;
        }

        public static IApplicationBuilder UseMvcConfiguration(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

            return app;
        }
    }
}