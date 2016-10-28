using System;

namespace Greentube.Monitoring
{
    /// <summary>
    /// An event raised by a Resource Monitor
    /// </summary>
    /// <seealso cref="IResourceMonitor"/>
    public interface IResourceMonitorEvent
    {
        /// <summary>
        /// Gets the verification time UTC.
        /// </summary>
        /// <value>
        /// The verification time UTC.
        /// </value>
        [PublicAPI]
        DateTime VerificationTimeUtc { get; }
        /// <summary>
        /// Gets a value indicating whether this resource is up.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is up; otherwise, <c>false</c>.
        /// </value>
        bool IsUp { get; }
        /// <summary>
        /// Gets the latency of the health check event
        /// </summary>
        /// <value>
        /// The latency.
        /// </value>
        [PublicAPI]
        TimeSpan Latency { get; }
        /// <summary>
        /// Gets the exception, in case one was thrown.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        [PublicAPI]
        Exception Exception { get; }
    }
}