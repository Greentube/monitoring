using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Greentube.Monitoring.SqlDb
{
    /// <summary>
    /// Health Check using SELECT 1
    /// </summary>
    /// <seealso cref="IHealthCheckStrategy" />
    public class SqlDbPingHealthCheckStrategy : IHealthCheckStrategy
    {
        private readonly IDbConnectionProvider _dbConnectionProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDbPingHealthCheckStrategy"/> class.
        /// </summary>
        /// <param name="dbConnectionProvider">The database connection provider.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <seealso cref="IDbConnectionProvider" />
        public SqlDbPingHealthCheckStrategy(IDbConnectionProvider dbConnectionProvider)
        {
            if (dbConnectionProvider == null) throw new ArgumentNullException(nameof(dbConnectionProvider));
            _dbConnectionProvider = dbConnectionProvider;
        }

        /// <summary>
        /// Checks the db connection with SELECT 1
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public async Task<bool> Check(CancellationToken token)
        {
            using (var connection = _dbConnectionProvider.GetDbConnection())
            {
                await connection.OpenAsync(token).ConfigureAwait(false);
                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT 1";
                    await cmd.ExecuteNonQueryAsync(token).ConfigureAwait(false);
                    return true;
                }
            }
        }
    }
}