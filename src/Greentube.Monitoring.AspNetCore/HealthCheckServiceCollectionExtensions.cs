using System;
using System.Collections.Generic;
using Greentube.Monitoring;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace - To ease discoverability (and avoid tons of using directives) on ASP.NET Core: Startup.cs
namespace Microsoft.Extensions.DependencyInjection
{
    public static class HealthCheckServiceCollectionExtensions
    {
        public static void AddMonitoring(this IServiceCollection services, Action<MonitoringOptions> optionsAction = null)
        {
            var options = new MonitoringOptions();
            optionsAction?.Invoke(options);

            services.AddSingleton<IResourceStateCollector>(p =>
            {
                var resourceMonitors = p.GetRequiredService<IEnumerable<IResourceMonitor>>();
                var logger = p.GetRequiredService<ILogger<ResourceStateCollector>>();
                return new ResourceStateCollector(resourceMonitors, options.MaxStatePerResource, logger);
            });
        }
    }

    public class MonitoringOptions
    {
        public int MaxStatePerResource { get; set; }
    }
}
