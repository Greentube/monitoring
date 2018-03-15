using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace Greentube.Monitoring.IntegrationTests
{
    public class ShutdownMonitorTests
    {
        private readonly TestServer _server;

        public ShutdownMonitorTests()
        {
            var webHostBuilder = new WebHostBuilder();
            _server = new TestServer(webHostBuilder.UseStartup<TestStartup>());
        }

        [Fact]
        public async Task Shutdown_Causes_IsNodeDown()
        {
            var client = _server.CreateClient();

            // check if node is up
            var httpResponseMessage = await client.GetAsync("/health");
            var healthCheckResponse = JsonConvert.DeserializeObject<HealthCheckResponse>(await httpResponseMessage.Content.ReadAsStringAsync());
            Assert.True(healthCheckResponse.Up);

            // bring down the monitor
            _server.Host.Services.GetRequiredService<IShutdownStatusProvider>().Shutdown();

            // wait to pick up changes
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            
            // check if node is down
            httpResponseMessage = await client.GetAsync("/health");
            healthCheckResponse = JsonConvert.DeserializeObject<HealthCheckResponse>(await httpResponseMessage.Content.ReadAsStringAsync());
            Assert.False(healthCheckResponse.Up);

        }

        private class HealthCheckResponse
        {
            public bool Up { get; set; }
        }
    }
}