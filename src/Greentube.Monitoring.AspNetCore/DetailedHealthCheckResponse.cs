using System.Collections.Generic;

namespace Greentube.Monitoring.AspNetCore
{
    /// <summary>
    /// Detailed Health Check response
    /// </summary>
    public class DetailedHealthCheckResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is up.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is up; otherwise, <c>false</c>.
        /// </value>
        public bool IsUp { get; set; }
        /// <summary>
        /// Gets or sets the resource states.
        /// </summary>
        /// <value>
        /// The resource states.
        /// </value>
        public IEnumerable<ResourceHealthStatus> ResourceStates { get; set; }
    }
}