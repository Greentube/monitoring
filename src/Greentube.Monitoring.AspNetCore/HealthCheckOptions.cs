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
    }
}