using Microsoft.Extensions.Logging;

namespace Greentube.Monitoring
{
    /// <summary>
    /// The Critical Monitor
    /// Usage: forcibly tell LoadBalancer to return {"Up":false} when application shutdowns to provide the graceful shutdown
    /// </summary>
    internal sealed class ShutdownMonitor : ResourceMonitor
    {
        public ShutdownMonitor(string resourceName, IShutdownStatusProvider shutdownStatusProvider, IResourceMonitorConfiguration configuration, ILogger<ResourceMonitor> logger) : base(resourceName, new ShutdownHealthCheckStrategy(shutdownStatusProvider), configuration, logger, true)
        {
        }
    }
}
