using System;
using Greentube.Monitoring;
using Greentube.Monitoring.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace - To ease discoverability (and avoid tons of using directives) on ASP.NET Core: Startup.cs
namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Monitoring Health check middleware
    /// </summary>
    public static class MonitoringEndpointApplicationBuilderExtensions
    {
        /// <summary>
        /// Uses the monitoring health check endpoint.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="optionsSetup">The options setup.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [PublicAPI]
        public static IApplicationBuilder UseMonitoringEndpoint(
            this IApplicationBuilder app,
            Action<HealthCheckOptions> optionsSetup = null)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            var provider = app.ApplicationServices;

            var collector = provider.GetRequiredService<IResourceStateCollector>();
            collector.Start();
            var lifetime = provider.GetService<IApplicationLifetime>();
            lifetime?.ApplicationStopping.Register(() => collector.Stop());

            var options = new HealthCheckOptions();
            optionsSetup?.Invoke(options);

            return app.Map(
                options.Path,
                m => m.UseMiddleware<HealthCheckMiddleware>(collector));
        }
    }
}
