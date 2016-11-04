using System;
using Greentube.Monitoring;
using Greentube.Monitoring.MongoDB;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// MongoDB extensions for MonitoringOptions
    /// </summary>
    public static class MongoDbMonitoringOptionsExtensions
    {
        /// <summary>
        /// Adds the MongoDB monitor to MonitoringOptions
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="mongo">The MongoDB.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="isCritical">if set to <c>true</c> [is critical].</param>
        /// <param name="configOverride">The configuration override.</param>
        public static void AddMongoDbMonitor(
            this MonitoringOptions options,
            IMongoDatabase mongo = null,
            string resourceName = "MongoDB",
            bool isCritical = true,
            IResourceMonitorConfiguration configOverride = null)
        {
            options.AddResourceMonitor((conf, provider) =>
            {
                var logger = provider.GetRequiredService<ILogger<MongoDbPingMonitor>>();
                mongo = mongo ?? provider.GetService<IMongoDatabase>();
                if (mongo == null)
                    throw new InvalidOperationException("A MongoDB instance is required either as an argument or from the Container.");

                return new MongoDbPingMonitor(
                    mongo,
                    logger,
                    configOverride ?? conf,
                    resourceName,
                    isCritical);
            });
        }
    }
}
