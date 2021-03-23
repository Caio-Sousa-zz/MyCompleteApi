using Elmah.Io.AspNetCore;
using Elmah.Io.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace DevIO.Api.Configuration
{
    public static class LoggerConfig
    {
        public static IServiceCollection AddLoggingConfig(this IServiceCollection services)
        {
            services.AddElmahIo(o =>
            {
                o.ApiKey = "20c02ea6a6cf4377bcde8df3e61769de";
                o.LogId = new Guid("8810c3c2-98e2-4900-bf89-d4e9a05b3fb1");
            });

            //services.AddLogging(b =>
            //{
            //    b.AddElmahIo(o =>
            //    {
            //        o.ApiKey = "20c02ea6a6cf4377bcde8df3e61769de";
            //        o.LogId = new Guid("8810c3c2-98e2-4900-bf89-d4e9a05b3fb1");
            //    });

            //    b.AddFilter<ElmahIoLoggerProvider>(null, LogLevel.Warning);
            //});

            return services;
        }

        public static IApplicationBuilder UseLoggingConfig(this IApplicationBuilder app)
        {
            app.UseElmahIo();

            return app;
        }
    }
}