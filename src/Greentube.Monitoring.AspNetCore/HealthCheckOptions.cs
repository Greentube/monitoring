namespace Greentube.Monitoring.AspNetCore
{
    /// <summary>
    /// Options for the health check endpoint
    /// </summary>
    public class HealthCheckOptions
    {
        /// <summary>
        /// Gets or sets the path to bind the Health Check
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public string Path { get; [PublicAPI] set; } = "/health";

        /// <summary>
        /// Gets or sets a value indicating whether [include version information].
        /// </summary>
        /// <value>
        /// <c>true</c> if [include version information]; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeVersionInformation { get; [PublicAPI] set; } = true;
    }
}