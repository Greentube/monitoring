using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Greentube.Monitoring.SqlDb
{
    /// <summary>
    /// Abstraction for creating database connection
    /// </summary>
    public interface IDbConnectionProvider
    {
        /// <summary>
        /// Provides database connection
        /// </summary>
        /// <returns>Database Connection</returns>
        DbConnection GetDbConnection();
    }

    /// <summary>
    /// Provides new db connection based on connection string
    /// </summary>
    public class DbConnectionProvider : IDbConnectionProvider
    {
        private readonly string _connectionString;

        /// <summary>
        /// Provides new db connection based on connection string
        /// </summary>
        /// <param name="connectionString">connection string used to create dbConnection</param>
        public DbConnectionProvider(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(_connectionString)) throw new ArgumentNullException(nameof(connectionString));
            _connectionString = connectionString;
        }

        /// <inheritdoc />
        public DbConnection GetDbConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}