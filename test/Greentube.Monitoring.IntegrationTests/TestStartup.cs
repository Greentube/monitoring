using System;
using System.Reflection;
using Greentube.Monitoring.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Greentube.Monitoring.IntegrationTests
{
    public sealed class TestStartup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMonitoring(Assembly.GetEntryAssembly(), options =>
            {
                options.AddShutdownMonitor(configOverride:new ResourceMonitorConfiguration(true, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100)));
            });
            return services.BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMonitoringEndpoint(options => options.Path = "/health");
        }
    }
}
