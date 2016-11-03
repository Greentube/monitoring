namespace Greentube.Monitoring.AspNetCore
{
    /// <summary>
    /// Health Check Middleware Options
    /// </summary>
    public class HealthCheckEndPointOptions
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