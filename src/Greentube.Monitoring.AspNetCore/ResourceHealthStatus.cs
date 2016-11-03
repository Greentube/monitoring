using System;
using System.Collections.Generic;

namespace Greentube.Monitoring.AspNetCore
{
    /// <summary>
    /// A detailed Health Check status
    /// </summary>
    public class ResourceHealthStatus
    {
        /// <summary>
        /// Gets or sets the name of the resource.
        /// </summary>
        /// <value>
        /// The name of the resource.
        /// </value>
        public string ResourceName { [UsedImplicitly] get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this resource is critical for the functioning of the system
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is critical; otherwise, <c>false</c>.
        /// </value>
        public bool IsCritical { [UsedImplicitly] get; set; }
        /// <summary>
        /// Gets or sets the events.
        /// </summary>
        /// <value>
        /// The events.
        /// </value>
        public IEnumerable<Event> Events { [UsedImplicitly] get; set; }

        /// <summary>
        /// An event raised by a health check process
        /// </summary>
        public class Event
        {
            /// <summary>
            /// Gets or sets a value indicating whether this node was up at the time of the check.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance is up; otherwise, <c>false</c>.
            /// </value>
            public bool IsUp { [UsedImplicitly]  get; set; }
            /// <summary>
            /// Gets or sets the verification time in UTC.
            /// </summary>
            /// <value>
            /// The verification time UTC.
            /// </value>
            public DateTime VerificationTimeUtc { [UsedImplicitly] get; set; }
            /// <summary>
            /// Gets or sets the latency of the health check
            /// </summary>
            /// <value>
            /// The latency.
            /// </value>
            public TimeSpan Latency { [UsedImplicitly] get; set; }
            /// <summary>
            /// Gets or sets the exception in case there was an error
            /// </summary>
            /// <value>
            /// The exception.
            /// </value>
            public Exception Exception { [UsedImplicitly] get; set; }
        }
    }
}
