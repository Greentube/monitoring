using Apache.NMS;
using Microsoft.Extensions.Logging;

namespace Greentube.Monitoring.Apache.NMS.ActiveMq
{
    /// <summary>
    /// Apache NMS Active MQ ping monitor
    /// </summary>
    public class ActiveMqPingMonitor : ResourceMonitor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveMqPingMonitor"/> class.
        /// </summary>
        /// <param name="resourceName">Name of monitored resource</param>
        /// <param name="connectionFactory">Factory of ActiveMq specific type of connection</param>
        /// <param name="logger">The logger.</param>
        /// <param name="queueName">Name of queue to connect on ping</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="isCritical">if set to <c>true</c> [is critical Resource].</param>
        public ActiveMqPingMonitor(
            string resourceName, 
            IConnectionFactory connectionFactory, 
            string queueName,
            IResourceMonitorConfiguration configuration, 
            ILogger<ResourceMonitor> logger, 
            bool isCritical = false)
            : base(
                  resourceName ?? "ActiveMQ", 
                  new ActiveMqPingHealthCheckStrategy(connectionFactory, queueName), 
                  configuration, 
                  logger, 
                  isCritical)
        {
        }
    }
}