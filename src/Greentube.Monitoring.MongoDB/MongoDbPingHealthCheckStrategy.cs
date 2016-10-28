using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Greentube.Monitoring.MongoDB
{
    /// <summary>
    /// Health Check using Ping command
    /// </summary>
    /// <remarks>
    /// Not suitable for Replica Set: A replica set status shall be checked.
    /// Ping could be successful in one of the nodes but the replica set is broken
    /// </remarks>
    /// <seealso cref="IHealthCheckStrategy" />
    public class MongoDbPingHealthCheckStrategy : IHealthCheckStrategy
    {
        private readonly IMongoDatabase _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDbPingHealthCheckStrategy"/> class.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public MongoDbPingHealthCheckStrategy(IMongoDatabase database)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));
            _database = database;
        }

        /// <summary>
        /// Checks connectivity with MongoDB via ping command
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public async Task<bool> Check(CancellationToken token)
        {
            await _database.RunCommandAsync((Command<BsonDocument>)"{ping:1}", cancellationToken: token)
                .ConfigureAwait(false);

            return true;
        }
    }
}