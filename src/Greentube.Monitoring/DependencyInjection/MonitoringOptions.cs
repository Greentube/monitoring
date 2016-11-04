using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Greentube.Monitoring
{
    /// <summary>
    /// Options for Monitoring services
    /// </summary>
    /// <seealso cref="Greentube.Monitoring.IResourceMonitorConfiguration" />
    public class MonitoringOptions : IResourceMonitorConfiguration
    {
        private readonly List<Func<IResourceMonitorConfiguration, IServiceProvider, IResourceMonitor>> _factories
            = new List<Func<IResourceMonitorConfiguration, IServiceProvider, IResourceMonitor>>();

        internal IEnumerable<Func<IResourceMonitorConfiguration, IServiceProvider, IResourceMonitor>> Factories => _factories;

        /// <summary>
        /// Gets or sets the maximum state instances per resource the Collector should keep.
        /// </summary>
        /// <value>
        /// The maximum state per resource.
        /// </value>
        public int MaxStatePerResource { get; [PublicAPI] set; } = 10;

        /// <summary>
        /// Gets or sets the verification interval when resource is down
        /// </summary>
        /// <value>
        /// The interval when down.
        /// </value>
        public TimeSpan IntervalWhenDown { get; [PublicAPI] set; } = TimeSpan.FromSeconds(15);

        /// <summary>
        /// Gets or sets the verification interval when resource is up.
        /// </summary>
        /// <value>
        /// The interval when up.
        /// </value>
        public TimeSpan IntervalWhenUp { get; [PublicAPI] set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Gets the timeout for a Monitor verification
        /// </summary>
        /// <value>
        /// The timeout.
        /// </value>
        public TimeSpan Timeout { get; [PublicAPI] set; } = TimeSpan.FromSeconds(3);

        /// <summary>
        /// Gets or sets a value indicating whether [run first check synchronously].
        /// </summary>
        /// <remarks>
        /// This flag instructs the Monitors to run the first check synchronously (Starting the Monitor becomes a blocking call)
        /// E.g: When running integration tests, waiting for Timers to trigger the Monitor delays the tests
        /// </remarks>
        /// <value>
        /// <c>true</c> if [run first check synchronously]; otherwise, <c>false</c>.
        /// </value>
        public bool StartSynchronously { get; [PublicAPI] set; } = false;

        /// <summary>
        /// Adds the resource monitor factory to be executed when ServiceProvider is available.
        /// </summary>
        /// <seealso cref="MonitoringServiceCollectionExtensions"/>
        /// <param name="factory">The factory.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [PublicAPI]
        public void AddResourceMonitor(Func<IResourceMonitorConfiguration, IServiceProvider, IResourceMonitor> factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            _factories.Add(factory);
        }
    }
}
