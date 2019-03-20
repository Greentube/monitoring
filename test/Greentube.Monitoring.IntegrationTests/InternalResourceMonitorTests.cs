using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace Greentube.Monitoring.IntegrationTests
{
    public class InternalResourceMonitorTests
    {
        private readonly TestServer _server;

        public InternalResourceMonitorTests()
        {
            var webHostBuilder = new WebHostBuilder();
            _server = new TestServer(webHostBuilder.UseStartup<TestStartup>());
        }

        [Fact]
        public async Task InternalStateIsMonitored()
        {
            var client = _server.CreateClient();

            // check if node is up
            var healthCheckResponse = await GetHealth(client);
            Assert.True(healthCheckResponse.Up);
            
            // set internalState to down
            await client.GetAsync("/?SetToDown");

            await Task.Delay(TimeSpan.FromMilliseconds(500));
            // check if node is down
            healthCheckResponse = await GetHealth(client);
            Assert.False(healthCheckResponse.Up);

            // set internalState to down
            await client.GetAsync("/?SetToUp");

            await Task.Delay(TimeSpan.FromMilliseconds(500));
            // check if node is up again
            healthCheckResponse = await GetHealth(client);
            Assert.True(healthCheckResponse.Up);
        }

        private static async Task<HealthCheckResponse> GetHealth(HttpClient client)
        {
            var httpResponseMessage = await client.GetAsync("/health?Detailed");
            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            var healthCheckResponse =
                JsonConvert.DeserializeObject<HealthCheckResponse>(content);
            return healthCheckResponse;
        }

        private class HealthCheckResponse
        {
            public bool Up { get; set; }
        }
    }
}