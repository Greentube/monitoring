using System.Threading;

namespace Greentube.Monitoring.Threading
{
    /// <summary>
    /// Creates TimerAdapter
    /// </summary>
    /// <remarks>
    /// This class is thread-safe
    /// </remarks>
    /// <seealso cref="ITimerFactory" />
    internal sealed class TimerAdapterFactory : ITimerFactory
    {
        /// <summary>
        /// Creates a Timer with the specified callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <returns></returns>
        public ITimer Create(TimerCallback callback)
        {
            return new TimerAdapter(callback);
        }
    }
}
