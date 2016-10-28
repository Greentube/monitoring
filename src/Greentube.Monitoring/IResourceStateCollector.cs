using System.Collections.Generic;
using Greentube.Monitoring.Threading;

namespace Greentube.Monitoring
{
    /// <summary>
    /// Resource State Collector
    /// </summary>
    /// <remarks>
    /// Collects <see cref="IResourceCurrentState"/> from its <see cref="IResourceMonitor"/>
    /// </remarks>
    /// <seealso cref="IResourceCurrentState" />
    /// <seealso cref="IResourceMonitor" />
    /// <seealso cref="IStartable" />
    public interface IResourceStateCollector : IStartable
    {
        /// <summary>
        /// Gets the maximum state per resource.
        /// </summary>
        /// <value>
        /// The maximum state per resource.
        /// </value>
        [PublicAPI]
        int MaxStatePerResource { get; }

        /// <summary>
        /// Gets the Collected Resources states
        /// </summary>
        /// <returns></returns>
        IEnumerable<IResourceCurrentState> GetStates();

        /// <summary>
        /// Adds a new Monitor to the Collector
        /// </summary>
        /// <param name="resourceMonitor">The monitor.</param>
        void AddMonitor(IResourceMonitor resourceMonitor);

        /// <summary>
        /// Removes a monitor from the Collector
        /// </summary>
        /// <param name="resourceMonitor">The monitor.</param>
        void RemoveMonitor(IResourceMonitor resourceMonitor);
    }
}