using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace Greentube.Monitoring
{
    /// <summary>
    /// Health check based on successful status code
    /// </summary>
    /// <seealso cref="Greentube.Monitoring.ResourceMonitor" />
    [PublicAPI]
    public class HttpResourceMonitor : ResourceMonitor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResourceMonitor"/> class.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="healthCheckEndpoint">The health check endpoint.</param>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="isCritical">if set to <c>true</c> [is critical].</param>
        public HttpResourceMonitor(
            string resourceName,
            Uri healthCheckEndpoint,
            HttpClient httpClient,
            ResourceMonitorConfiguration configuration,
            ILogger<HttpResourceMonitor> logger,
            bool isCritical = false)
            : base(
                  resourceName,
                  new HttpSuccessStatusCodeHealthCheckStrategy(httpClient, healthCheckEndpoint),
                  configuration,
                  logger,
                  isCritical)
        {
        }
    }
}
