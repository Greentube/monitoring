using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Greentube.Monitoring.SqlDb
{
    /// <summary>
    /// Sql Monitoring extensions for MonitoringOptions
    /// </summary>
    public static class SqlDbMonitoringOptionsExtensions
    {
        /// <summary>
        /// Adds the sql database monitor to MonitoringOptions
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="dbProvider">Database provider</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="isCritical">if set to <c>true</c> [is critical].</param>
        /// <param name="configOverride">The configuration override.</param>
        public static void AddSqlDbMonitor(
            this MonitoringOptions options,
            IDbConnectionProvider dbProvider,
            string resourceName = null,
            bool isCritical = true,
            IResourceMonitorConfiguration configOverride = null)
        {
            options.AddResourceMonitor((conf, provider) =>
            {
                var logger = provider.GetRequiredService<ILogger<SqlDbPingMonitor>>();

                return new SqlDbPingMonitor(
                    dbProvider,
                    logger,
                    configOverride ?? conf,
                    resourceName ?? "SQL Database",
                    isCritical);
            });
        }

        /// <summary>
        /// Adds the sql database monitor to MonitoringOptions
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="dbConnectionString">Database connection string</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="isCritical">if set to <c>true</c> [is critical].</param>
        /// <param name="configOverride">The configuration override.</param>
        public static void AddSqlDbMonitor(
            this MonitoringOptions options,
            string dbConnectionString,
            string resourceName = null,
            bool isCritical = true,
            IResourceMonitorConfiguration configOverride = null)
        {
            options.AddSqlDbMonitor(
                new DbConnectionProvider(dbConnectionString), 
                resourceName, 
                isCritical, 
                configOverride);
        }
    }
}