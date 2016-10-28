using System.Collections.Generic;

namespace Greentube.Monitoring
{
    /// <summary>
    /// Represents an External Resources' current state
    /// </summary>
    /// <seealso cref="IResourceMonitor"/>
    public interface IResourceCurrentState
    {
        /// <summary>
        /// Gets a value indicating whether this resource is up.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is up; otherwise, <c>false</c>.
        /// </value>
        bool IsUp { get; }
        /// <summary>
        /// Gets the resource monitor which does the actual check and reports state
        /// </summary>
        /// <value>
        /// The resource monitor.
        /// </value>
        IResourceMonitor ResourceMonitor { get; }
        /// <summary>
        /// Gets the Monitor Events for this resource
        /// </summary>
        /// <value>
        /// The monitor events.
        /// </value>
        IEnumerable<IResourceMonitorEvent> MonitorEvents { get; }
    }
}