using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Greentube.Monitoring.DependencyInjection
{
    public static class ShutdownMonitoringExtensions
    {
        /// <summary>
        /// Adds the Shutdown monitor.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="resourceName">Name of the resource. Defaults to: Uri.AbsoluteUri.</param>
        /// <param name="configOverride">The configuration override.</param>
        public static void AddShutdownMonitor(
            this MonitoringOptions options,

            string resourceName = null,
            IResourceMonitorConfiguration configOverride = null)
        {
            options.AddResourceMonitor((conf, provider) =>
            {
                var logger = provider.GetRequiredService<ILogger<ShutdownMonitor>>();
                return new ShutdownMonitor(
                    resourceName ?? "shutdown",
                    provider.GetRequiredService<IShutdownStatusProvider>(),
                    configOverride ?? conf,
                    logger
                );
            });
        }
    }
}
