using Microsoft.Extensions.Logging;

namespace Greentube.Monitoring.SqlDb
{
    /// <summary>
    /// Sql Database ping monitor
    /// </summary>
    /// <seealso cref="Greentube.Monitoring.ResourceMonitor" />
    public sealed class SqlDbPingMonitor : ResourceMonitor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDbPingMonitor"/> class.
        /// </summary>
        /// <param name="isCritical">if set to <c>true</c> [is critical Resource].</param>
        /// <param name="connectionProvider">The connection provider.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="resourceName">Name of the resource.</param>
        public SqlDbPingMonitor(
            IDbConnectionProvider connectionProvider,
            ILogger<ResourceMonitor> logger,
            IResourceMonitorConfiguration configuration, 
            string resourceName = null, 
            bool isCritical = false) 
            : base(
                  resourceName,
                  new SqlDbPingHealthCheckStrategy(connectionProvider), 
                  configuration, 
                  logger, 
                  isCritical)
        { }
    }
}