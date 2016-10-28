using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Greentube.Monitoring.Redis
{
    /// <summary>
    /// Redis ping monitor
    /// </summary>
    /// <seealso cref="Greentube.Monitoring.ResourceMonitor" />
    public sealed class RedisPingMonitor : ResourceMonitor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedisPingMonitor"/> class.
        /// </summary>
        /// <param name="isCritical">if set to <c>true</c> [is critical Resource].</param>
        /// <param name="connectionMultiplexer">The connection multiplexer.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="resourceName">Name of the resource.</param>
        public RedisPingMonitor(
            bool isCritical,
            IConnectionMultiplexer connectionMultiplexer,
            ILogger<RedisPingMonitor> logger,
            ResourceMonitorConfiguration configuration,
            string resourceName = null)
            : base(resourceName ?? GetResourceName(connectionMultiplexer),
              new RedisPingHealthCheckStrategy(connectionMultiplexer),
              configuration,
              logger,
              isCritical)
        {
        }

        private static string GetResourceName(IConnectionMultiplexer connectionMultiplexer)
        {
            if (connectionMultiplexer == null) throw new ArgumentNullException(nameof(connectionMultiplexer));

            return "Redis:" + string.Join(",",
                ConfigurationOptions.Parse(connectionMultiplexer.Configuration)
                .EndPoints.Select(e => e.ToString()));
        }
    }
}
