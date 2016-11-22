using System;
using Apache.NMS.ActiveMQ;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Greentube.Monitoring.Apache.NMS.ActiveMq
{
    /// <summary>
    /// ActiveMQ extensions for MonitoringOptions
    /// </summary>
    public static class ActiveMqMonitoringOptionsExtensions
    {
        /// <summary>
        /// Adds monitoring of ActiveMQ
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="configFactory">Function that produce configuration for monitoring connection <seealso cref="IActiveMqMonitoringConfig"/></param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="isCritical">if set to <c>true</c> [is critical].</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddActiveMqMonitor(this MonitoringOptions options, Func<IResourceMonitorConfiguration, IServiceProvider, IActiveMqMonitoringConfig> configFactory, string resourceName = null, bool isCritical = true)
        {
            
            options.AddResourceMonitor((configuration, provider) =>
            {
                var config = configFactory(configuration, provider);

                if (config.Url == null) throw new ArgumentNullException(nameof(config.Url));

                var connectionFactory = new ConnectionFactory()
                {
                    BrokerUri = new Uri(config.Url),
                    UserName = config.User,
                    Password = config.Password
                };
                return new ActiveMqPingMonitor(resourceName, connectionFactory, configuration, provider.GetRequiredService<ILogger<ActiveMqPingMonitor>>(), isCritical);
            });
        }

        /// <summary>
        /// Adds monitoring of ActiveMQ
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="url">Url used to connect to ActiveMQ</param>
        /// <param name="username">User used to connect to ActiveMQ</param>
        /// <param name="password">Password used to connect to ActiveMQ</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="isCritical">if set to <c>true</c> [is critical].</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddActiveMqMonitor(this MonitoringOptions options, string url, string username, string password, string resourceName = null, bool isCritical = false)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            options.AddResourceMonitor(
                (configuration, provider) => new ActiveMqPingMonitor(resourceName, new ConnectionFactory()
                {
                    BrokerUri = new Uri(url),
                    UserName = username,
                    Password = password
                }, configuration, provider.GetRequiredService<ILogger<ActiveMqPingMonitor>>(), isCritical));
        }
    }
}
