using System;
using System.Reflection;
using Greentube.Monitoring.DependencyInjection;
using Greentube.Monitoring.InternalResource;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;


namespace Greentube.Monitoring.IntegrationTests
{
    public sealed class TestStartup
    {
        private SimpleInternalResource _testInternalResource;
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            _testInternalResource = new SimpleInternalResource { IsUp = true };
            services.AddMonitoring(Assembly.GetEntryAssembly(), options =>
            {
                options.AddShutdownMonitor(configOverride:new ResourceMonitorConfiguration(true, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100)));
                options.AddInternalResourceMonitor(_testInternalResource, "simple resource", configOverride: new ResourceMonitorConfiguration(true, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100)), isCritical: true);
            });
            return services.BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMonitoringEndpoint(options => options.Path = "/health");

            app.Run(async context =>
            {
                if (context.Request.Query.ContainsKey("SetToUp"))
                    { _testInternalResource.IsUp = true;}

                if (context.Request.Query.ContainsKey("SetToDown"))
                    { _testInternalResource.IsUp = false; }
            });
        }
    }
}
