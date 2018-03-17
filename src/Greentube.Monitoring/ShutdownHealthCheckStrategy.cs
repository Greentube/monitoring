using System.Threading;
using System.Threading.Tasks;

namespace Greentube.Monitoring
{
    internal sealed class ShutdownHealthCheckStrategy : IHealthCheckStrategy
    {
        private readonly IShutdownStatusProvider _shutdownStatusProvider;

        public ShutdownHealthCheckStrategy(IShutdownStatusProvider shutdownStatusProvider)
        {
            _shutdownStatusProvider = shutdownStatusProvider;
        }

        public Task<bool> Check(CancellationToken token)
        {
            return Task.FromResult(!_shutdownStatusProvider.IsShuttingDown);
        }
    }
}
