using System;
using System.Net.Http;
using Greentube.Monitoring;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// HTTP extensions for MonitoringOptions
    /// </summary>
    public static class HttpMonitoringOptionsExtensions
    {
        /// <summary>
        /// Adds the HTTP monitor.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="healthCheckEndpoint">The health check endpoint.</param>
        /// <param name="resourceName">Name of the resource. Defaults to: Uri.AbsoluteUri.</param>
        /// <param name="isCritical">if set to <c>true</c> [is critical].</param>
        /// <param name="configOverride">The configuration override.</param>
        public static void AddHttpMonitor(
            this MonitoringOptions options,
            Uri healthCheckEndpoint,
            string resourceName = null,
            bool isCritical = true,
            IResourceMonitorConfiguration configOverride = null)
        {
            resourceName = resourceName ?? healthCheckEndpoint.AbsoluteUri; // Defaults to the AbsoluteUri of the Health Check Endpoint
            options.AddResourceMonitor((conf, provider) =>
            {
                var logger = provider.GetRequiredService<ILogger<HttpResourceMonitor>>();
                return new HttpResourceMonitor(
                    resourceName,
                    healthCheckEndpoint,
                    new HttpClient(),
                    configOverride ?? conf,
                    logger,
                    isCritical);
            });
        }
    }
}
