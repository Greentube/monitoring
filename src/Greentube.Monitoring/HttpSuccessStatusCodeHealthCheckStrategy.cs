using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Greentube.Monitoring
{
    /// <summary>
    /// HTTP health check based on successful status code
    /// </summary>
    /// <seealso cref="IHealthCheckStrategy" />
    /// <seealso href="https://msdn.microsoft.com/en-us/library/system.net.http.httpresponsemessage.ensuresuccessstatuscode.aspx"/>
    public class HttpSuccessStatusCodeHealthCheckStrategy : IHealthCheckStrategy
    {
        private readonly HttpClient _client;
        private readonly Uri _endpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpSuccessStatusCodeHealthCheckStrategy"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public HttpSuccessStatusCodeHealthCheckStrategy(HttpClient client, Uri endpoint)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));
            _client = client;
            _endpoint = endpoint;
        }

        /// <summary>
        /// Makes a request and throws <see cref="HttpRequestException"/> in case of error
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public async Task<bool> Check(CancellationToken token)
        {
            var check = await _client.GetAsync(_endpoint, token)
                .ConfigureAwait(false);

            check.EnsureSuccessStatusCode(); // Basic Health-check should return 200
            return true;
        }
    }
}