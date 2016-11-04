using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Greentube.Monitoring.AspNetCore
{
    /// <summary>
    /// AspNetCore health check middleware using <see cref="IResourceStateCollector"/>
    /// </summary>
    public class HealthCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IResourceStateCollector _collector;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthCheckMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next.</param>
        /// <param name="collector">The collector.</param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public HealthCheckMiddleware(RequestDelegate next, IResourceStateCollector collector)
        {
            if (next == null) throw new ArgumentNullException(nameof(next));
            if (collector == null) throw new ArgumentNullException(nameof(collector));
            _next = next;
            _collector = collector;
        }

        /// <summary>
        /// Invokes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method != "GET")
            {
                await _next(context);
                return;
            }

            var includeDetails = context.Request.Query.ContainsKey("detailed");
            if (includeDetails)
                await HandleDetailedHealthCheckRequest(context);
            else
                await HandleHealthCheckRequest(context);
        }

        private async Task HandleHealthCheckRequest(HttpContext context)
        {
            var isNodeUp = !_collector.GetStates().Any(IsNodeDownStrategy);

            var body = new { Up = isNodeUp };

            await WriteResponse(context, body, isNodeUp);
        }

        private async Task HandleDetailedHealthCheckRequest(HttpContext context)
        {
            var models = new List<ResourceHealthStatus>();

            var isNodeUp = true;
            foreach (var resourceState in _collector.GetStates())
            {
                if (IsNodeDownStrategy(resourceState))
                {
                    isNodeUp = false;
                }

                models.Add(new ResourceHealthStatus
                {
                    ResourceName = resourceState.ResourceMonitor.ResourceName,
                    IsCritical = resourceState.ResourceMonitor.IsCritical,
                    Events = resourceState
                        .MonitorEvents
                        .Select(e => new ResourceHealthStatus.Event
                        {
                            Exception = e.Exception?.ToString(),
                            Latency = e.Latency,
                            Up = e.IsUp,
                            VerificationTimeUtc = e.VerificationTimeUtc
                        })
                });
            }

            var body = new HealthCheckDetailedResponse
            {
                Up = isNodeUp,
                ResourceStates = models
            };

            await WriteResponse(context, body, isNodeUp);
        }

        private static bool IsNodeDownStrategy(IResourceCurrentState resourceState)
        {
            // In case of a Critical resource being down, we report Node Down
            return resourceState.ResourceMonitor.IsCritical && !resourceState.IsUp;
        }

        private static async Task WriteResponse(HttpContext context, object body, bool isNodeUp)
        {
            var responseBody = JsonConvert.SerializeObject(body);

            context.Response.ContentType = "application/json";
            context.Response.ContentLength = responseBody.Length;
            await context.Response.WriteAsync(responseBody);

            context.Response.StatusCode
                = isNodeUp
                    ? StatusCodes.Status200OK
                    : StatusCodes.Status503ServiceUnavailable;
        }
    }
}
