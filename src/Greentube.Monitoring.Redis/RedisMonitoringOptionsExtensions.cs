using System;
using Greentube.Monitoring;
using Greentube.Monitoring.Redis;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Redis extensions for MonitoringOptions
    /// </summary>
    public static class RedisMonitoringOptionsExtensions
    {
        /// <summary>
        /// Adds the redis monitor to MonitoringOptions
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="multiplexer">The multiplexer.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="isCritical">if set to <c>true</c> [is critical].</param>
        /// <param name="configOverride">The configuration override.</param>
        public static void AddRedisMonitor(
            this MonitoringOptions options,
            IConnectionMultiplexer multiplexer = null,
            string resourceName = "Redis",
            bool isCritical = true,
            IResourceMonitorConfiguration configOverride = null)
        {
            options.AddResourceMonitor((conf, provider) =>
            {
                var logger = provider.GetRequiredService<ILogger<RedisPingMonitor>>();
                multiplexer = multiplexer ?? provider.GetService<IConnectionMultiplexer>();
                if (multiplexer == null)
                    throw new InvalidOperationException("A multiplexer is required either as an argument or from the Container.");

                return new RedisPingMonitor(
                    isCritical,
                    multiplexer,
                    logger,
                    configOverride ?? conf,
                    resourceName);
            });
        }
    }
}
