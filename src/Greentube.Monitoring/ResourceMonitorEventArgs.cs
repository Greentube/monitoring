using System;

namespace Greentube.Monitoring
{
    /// <summary>
    /// Event args raised by a Resource Monitor
    /// </summary>
    /// <seealso cref="EventArgs" />
    public class ResourceMonitorEventArgs : EventArgs, IResourceMonitorEvent
    {
        /// <summary>
        /// Gets the verification time UTC.
        /// </summary>
        /// <value>
        /// The verification time UTC.
        /// </value>
        public DateTime VerificationTimeUtc { get; } = DateTime.UtcNow;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is up.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is up; otherwise, <c>false</c>.
        /// </value>
        public bool IsUp { get; set; }
        /// <summary>
        /// Gets or sets the latency.
        /// </summary>
        /// <value>
        /// The latency.
        /// </value>
        public TimeSpan Latency { get; set; }
        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get; set; }
    }
}