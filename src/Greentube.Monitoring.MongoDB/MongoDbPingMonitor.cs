using System.Linq;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Greentube.Monitoring.MongoDB
{
    /// <summary>
    /// Monitors MongoDB connectivity
    /// </summary>
    /// <seealso cref="ResourceMonitor" />
    public sealed class MongoDbPingMonitor : ResourceMonitor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDbPingMonitor"/> class.
        /// </summary>
        /// <param name="mongoDatabase">The mongo database.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="resourceName">Name of the resource (If not provided: will be based on Servers EndPoints).</param>
        /// <param name="isCritical">if set to <c>true</c> [is critical].</param>
        public MongoDbPingMonitor(
            IMongoDatabase mongoDatabase,
            ILogger<MongoDbPingMonitor> logger,
            ResourceMonitorConfiguration configuration,
            string resourceName = null,
            bool isCritical = true)
            : base(resourceName ?? "MongoDB:" + string.Join(",", mongoDatabase.Client.Settings.Servers.Select(e => e.ToString())),
              new MongoDbPingHealthCheckStrategy(mongoDatabase),
              configuration,
              logger,
              isCritical)
        {
        }
    }
}
