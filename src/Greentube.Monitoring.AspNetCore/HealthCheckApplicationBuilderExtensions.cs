using System;
using Greentube.Monitoring;
using Greentube.Monitoring.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace - To ease discoverability (and avoid tons of using directives) on ASP.NET Core: Startup.cs
namespace Microsoft.AspNetCore.Builder
{
    public static class HealthCheckApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseHealthCheckEndpoint(
        this IApplicationBuilder app,
        Action<HealthCheckOptions> optionsSetup = null)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            // Validate state
            var applicationServices = app.ApplicationServices;
            var collector = applicationServices.GetRequiredService<IResourceStateCollector>();

            var options = new HealthCheckOptions();
            optionsSetup?.Invoke(options);

            return app.Map(options.Path,
                m => m.UseMiddleware<HealthCheckMiddleware>(collector));
        }
    }
}
