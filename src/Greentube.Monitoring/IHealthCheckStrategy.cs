using System.Threading;
using System.Threading.Tasks;

namespace Greentube.Monitoring
{
    /// <summary>
    /// Health Check Strategy
    /// </summary>
    public interface IHealthCheckStrategy
    {
        /// <summary>
        /// Health check.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>true if resource is up or false (or throws in case there's context) when down</returns>
        Task<bool> Check(CancellationToken token);
    }
}
