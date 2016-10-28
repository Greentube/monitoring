using System;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Greentube.Monitoring.Redis
{
    /// <summary>
    /// HealthCheck based on Ping command
    /// </summary>
    /// <seealso cref="IHealthCheckStrategy" />
    public class RedisPingHealthCheckStrategy : IHealthCheckStrategy
    {
        private readonly IConnectionMultiplexer _multiplexer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisPingHealthCheckStrategy"/> class.
        /// </summary>
        /// <param name="multiplexer">The multiplexer.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public RedisPingHealthCheckStrategy(IConnectionMultiplexer multiplexer)
        {
            if (multiplexer == null) throw new ArgumentNullException(nameof(multiplexer));
            _multiplexer = multiplexer;
        }

        /// <summary>
        /// Checks the multiplexer is connected
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public async Task<bool> Check(CancellationToken token)
        {
            if (_multiplexer.IsConnected)
                return true;

            // If not connected, make a call to let exception bubble
            await _multiplexer
                .GetDatabase(0)
                .PingAsync(CommandFlags.DemandMaster)
                .ConfigureAwait(false);

            return true; // if it didn't throw, it's back up
        }
    }
}