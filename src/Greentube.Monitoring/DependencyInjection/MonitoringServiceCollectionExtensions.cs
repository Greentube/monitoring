using System;
using System.Linq;
using System.Reflection;
using Greentube.Monitoring;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace - To ease discoverability (and avoid tons of using directives) on ASP.NET Core: Startup.cs
namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Registers services required by Monitoring library
    /// </summary>
    public static class MonitoringServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the monitoring services
        /// </summary>
        /// <remarks>
        /// The Assembly parameter is not needed when targeting netstandard1.5
        /// When this package targets 1.5, we can delete this argument and call Assembly.GetEntryAssembly() instead
        /// </remarks>
        /// <param name="services">The services.</param>
        /// <param name="entryAssembly">Reference to the Entry Assembly (to take version from)</param>
        /// <param name="optionsAction">The options action.</param>
        /// <returns></returns>
        public static IServiceCollection AddMonitoring(
            this IServiceCollection services,
            Assembly entryAssembly,
            Action<MonitoringOptions> optionsAction = null)
        {
            var options = new MonitoringOptions();
            optionsAction?.Invoke(options);

            return services
                .AddSingleton<IVersionService, VersionService>(
                    provider => new VersionService(provider.GetRequiredService<IHostingEnvironment>(), entryAssembly))
                .AddSingleton<IResourceMonitorConfiguration>(options)
                .AddSingleton<IResourceStateCollector>(p =>
                {
                    var resourceMonitors = options
                        .Factories
                        .Select(f => f(options, p))
                        .Concat(p.GetServices<IResourceMonitor>());

                    var logger = p.GetRequiredService<ILogger<ResourceStateCollector>>();
                    return new ResourceStateCollector(resourceMonitors, options.MaxStatePerResource, logger);
                });
        }
    }
}
