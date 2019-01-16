using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Greentube.Monitoring.SqlDb
{
    /// <summary>
    /// Health Check calling specific stored procedure in checked DB and schema
    /// </summary>
    /// <seealso cref="T:Greentube.Monitoring.IHealthCheckStrategy" />
    public sealed class SqlDbStoredProcedureHealthCheckStrategy : IHealthCheckStrategy
    {
        private readonly IDbConnectionProvider _dbConnectionProvider;
        private readonly string _fullStoredProcedureName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDbStoredProcedureHealthCheckStrategy"/> class.
        /// </summary>
        /// <param name="dbConnectionProvider">The database connection provider.</param>
        /// <param name="fullStoredProcedureName">Full stored procedure name including schema.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <seealso cref="IDbConnectionProvider" />
        public SqlDbStoredProcedureHealthCheckStrategy(IDbConnectionProvider dbConnectionProvider, string fullStoredProcedureName)
        {
            _dbConnectionProvider = dbConnectionProvider ?? throw new ArgumentNullException(nameof(dbConnectionProvider));
            _fullStoredProcedureName = fullStoredProcedureName ?? throw new ArgumentNullException(nameof(fullStoredProcedureName));
        }

        /// <summary>
        /// Checks the DB connection by executing health check stored procedure
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> Check(CancellationToken token)
        {
            using (var connection = _dbConnectionProvider.GetDbConnection())
            {
                await connection.OpenAsync(token).ConfigureAwait(false);
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = _fullStoredProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cmd.ExecuteNonQueryAsync(token).ConfigureAwait(false);
                    return true;
                }
            }
        }
    }
}
