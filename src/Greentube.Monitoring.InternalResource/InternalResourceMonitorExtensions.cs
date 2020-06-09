using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Greentube.Monitoring.InternalResource
{
    public static class InternalResourceMonitoringExtensions
    {
        /// <summary>
        /// Adds the internal resource monitor.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="internalResource">Instance of internal resource that has state</param>
        /// <param name="resourceName">Name of the resource. Defaults to: Uri.AbsoluteUri.</param>
        /// <param name="isCritical">if set to <c>true</c> [is critical].</param>
        /// <param name="configOverride">The configuration override.</param>
        public static void AddInternalResourceMonitor(
            this MonitoringOptions options,
            IInternalResourceMonitored internalResource,
            string resourceName,
            bool isCritical = false,
            IResourceMonitorConfiguration configOverride = null)
        {
            options.AddResourceMonitor((conf, provider) =>
            {
                var logger = provider.GetRequiredService<ILogger<InternalResourceMonitor>>();
                return new InternalResourceMonitor(
                    resourceName,
                    new InternalResourceHealthCheckStrategy(internalResource), 
                    configOverride ?? conf,
                    logger, isCritical
                );
            });
        }
    }
}