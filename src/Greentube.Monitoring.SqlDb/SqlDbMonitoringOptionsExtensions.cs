using System.Data.SqlClient;
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

        /// <summary>
        /// Adds the SQL database stored procedure monitor to MonitoringOptions
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="dbConnectionProvider">Database connection provider.</param>
        /// <param name="schema">DB schema monitored.</param>
        /// <param name="storedProcedureName">Healthcheck stored procedure name (without schema).</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="isCritical">if set to <c>true</c> [is critical].</param>
        /// <param name="configOverride">The configuration override.</param>
        public static void AddSqlDbStoredProcedureMonitor(
            this MonitoringOptions options,
            IDbConnectionProvider dbConnectionProvider,
            string schema,
            string storedProcedureName,
            string resourceName,
            bool isCritical = true,
            IResourceMonitorConfiguration configOverride = null)
        {
            options.AddResourceMonitor((conf, provider) =>
            {
                var logger = provider.GetRequiredService<ILogger<SqlDbStoredProcedureMonitor>>();

                return new SqlDbStoredProcedureMonitor(
                    dbConnectionProvider,
                    schema,
                    storedProcedureName,
                    logger,
                    configOverride ?? conf,
                    resourceName,
                    isCritical);
            });
        }

        /// <summary>
        /// Adds the SQL database stored procedure monitor to MonitoringOptions
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="dbConnectionString">Database connection string.</param>
        /// <param name="schema">DB schema monitored.</param>
        /// <param name="storedProcedureName">Healthcheck stored procedure name (without schema).</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="isCritical">if set to <c>true</c> [is critical].</param>
        /// <param name="configOverride">The configuration override.</param>
        public static void AddSqlDbStoredProcedureMonitor(
            this MonitoringOptions options, 
            string dbConnectionString,
            string schema, 
            string storedProcedureName, 
            string resourceName = null, 
            bool isCritical = true,
            IResourceMonitorConfiguration configOverride = null)
        {
            options.AddSqlDbStoredProcedureMonitor(
                new DbConnectionProvider(dbConnectionString), 
                schema,
                storedProcedureName, 
                resourceName ?? DbSchemaResourceName(dbConnectionString, schema), 
                isCritical,
                configOverride);
        }

        /// <summary>
        /// helper function used to construct SQL stored procedure monitor resource name from DB connection string and schema
        /// </summary>
        /// <param name="dbConnectionString"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        public static string DbSchemaResourceName(string dbConnectionString, string schema)
        {
            var builder = new SqlConnectionStringBuilder(dbConnectionString);
            return $"{builder.InitialCatalog}.{schema}";
        }
    }
}