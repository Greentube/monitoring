using System.Collections.Generic;

namespace Greentube.Monitoring.AspNetCore
{
    /// <summary>
    /// Health check middleware detailed response
    /// </summary>
    public class HealthCheckDetailedResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether this node is up.
        /// </summary>
        /// <value>
        ///   <c>true</c> if up; otherwise, <c>false</c>.
        /// </value>
        public bool Up { get; set; }

        /// <summary>
        /// Gets or sets the version information.
        /// </summary>
        /// <value>
        /// The version information.
        /// </value>
        public VersionInformation VersionInformation { get; set; }

        /// <summary>
        /// Gets or sets the resource states.
        /// </summary>
        /// <value>
        /// The resource states.
        /// </value>
        public IEnumerable<ResourceHealthStatus> ResourceStates { get; set; }
    }
}
