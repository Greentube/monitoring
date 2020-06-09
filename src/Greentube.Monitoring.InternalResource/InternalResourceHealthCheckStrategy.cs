using System.Threading;
using System.Threading.Tasks;

namespace Greentube.Monitoring.InternalResource
{
    public sealed class InternalResourceHealthCheckStrategy : IHealthCheckStrategy
    {
        private readonly IInternalResourceMonitored _internalResourceMonitored;

        public InternalResourceHealthCheckStrategy(IInternalResourceMonitored internalResourceMonitored)
        {
            _internalResourceMonitored = internalResourceMonitored;
        }

        public Task<bool> Check(CancellationToken token)
        {
            return Task.FromResult(_internalResourceMonitored.IsUp);
        }
    }
}
