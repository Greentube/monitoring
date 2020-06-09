using Microsoft.Extensions.Logging;

namespace Greentube.Monitoring.InternalResource
{
    /// <summary>
    /// Internal resource monitor
    ///<seealso cref="ResourceMonitor" />
    /// </summary>
    public sealed class InternalResourceMonitor : ResourceMonitor
    {
        public InternalResourceMonitor(
            string resourceName,
            InternalResourceHealthCheckStrategy verificationStrategy,
            IResourceMonitorConfiguration configuration,
            ILogger<ResourceMonitor> logger,
            bool isCritical = false)
            : base(resourceName, verificationStrategy, configuration, logger, isCritical)
        {
        }
    }
}
