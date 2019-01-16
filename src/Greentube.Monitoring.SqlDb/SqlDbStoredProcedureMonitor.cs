using System;
using Microsoft.Extensions.Logging;

namespace Greentube.Monitoring.SqlDb
{
    /// <summary>
    /// Sql Database stored procedure monitor
    /// </summary>
    /// <seealso cref="ResourceMonitor" />
    public sealed class SqlDbStoredProcedureMonitor : ResourceMonitor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDbStoredProcedureMonitor"/> class.
        /// </summary>
        /// <param name="dbConnectionProvider">The DB connection provider.</param>
        /// <param name="schema">DB schema monitored.</param>
        /// <param name="storedProcedureName">Healthcheck stored procedure name (without schema).</param>
        /// <param name="logger">The logger.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="isCritical">if set to <c>true</c> [is critical Resource].</param>
        public SqlDbStoredProcedureMonitor(
            IDbConnectionProvider dbConnectionProvider,
            string schema,
            string storedProcedureName,
            ILogger<ResourceMonitor> logger,
            IResourceMonitorConfiguration configuration,
            string resourceName,
            bool isCritical = false)
            : base(
                resourceName,
                new SqlDbStoredProcedureHealthCheckStrategy(dbConnectionProvider,
                    $"{schema ?? throw new ArgumentNullException(nameof(schema))}.{storedProcedureName ?? throw new ArgumentNullException(nameof(storedProcedureName))}"),
                configuration,
                logger,
                isCritical)
        {
        }
    }
}
