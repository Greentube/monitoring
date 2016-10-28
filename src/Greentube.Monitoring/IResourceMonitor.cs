using System;
using Greentube.Monitoring.Threading;

namespace Greentube.Monitoring
{
    /// <summary>
    /// Monitors the state of an external resource
    /// </summary>
    public interface IResourceMonitor : IStartable
    {
        /// <summary>
        /// Gets the name of the resource.
        /// </summary>
        /// <value>
        /// The name of the resource.
        /// </value>
        string ResourceName { get; }
        /// <summary>
        /// Gets a value indicating whether this resource is critical to the functioning of the system
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is critical; otherwise, <c>false</c>.
        /// </value>
        [PublicAPI]
        bool IsCritical { get; }
        /// <summary>
        /// Occurs when a verification is executed
        /// </summary>
        event EventHandler<ResourceMonitorEventArgs> MonitoringEvent;
    }
}