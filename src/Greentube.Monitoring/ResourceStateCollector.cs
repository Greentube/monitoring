using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Greentube.Monitoring.Threading;
using Microsoft.Extensions.Logging;

namespace Greentube.Monitoring
{
    /// <summary>
    /// Keeps the last N events of each <see cref="IResourceMonitor"/>
    /// </summary>
    public class ResourceStateCollector : AbstractStartable, IResourceStateCollector
    {
        private readonly ILogger<ResourceStateCollector> _logger;
        private readonly ConcurrentDictionary<IResourceMonitor, ResourceCurrentState> _resourceStates;

        public int MaxStatePerResource { get; }

        public ResourceStateCollector(
            IEnumerable<IResourceMonitor> initialMonitors,
            int maxStatePerResource,
            ILogger<ResourceStateCollector> logger)
        {
            if (initialMonitors == null) throw new ArgumentNullException(nameof(initialMonitors));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            _logger = logger;
            MaxStatePerResource = maxStatePerResource;
            _resourceStates = new ConcurrentDictionary<IResourceMonitor, ResourceCurrentState>(
                initialMonitors.ToDictionary(r => r, m => new ResourceCurrentState(m, maxStatePerResource)));
        }

        /// <summary>
        /// Adds a new Monitor to the Collector
        /// </summary>
        /// <param name="resourceMonitor">The monitor.</param>
        public void AddMonitor(IResourceMonitor resourceMonitor)
        {
            var resourceState = new ResourceCurrentState(resourceMonitor, MaxStatePerResource);
            if (!_resourceStates.TryAdd(resourceMonitor, resourceState))
            {
                _logger.LogError("Couldn't add {Monitor} to collector.", resourceMonitor);
                return;
            }

            if (IsRunning)
                resourceState.Start();

            _logger.LogTrace("Added Resource Monitor {Monitor}. Collector running state: {IsCollectorRunning}, Resource running state: {IsResourceRunning}",
                resourceMonitor, IsRunning, resourceMonitor.IsRunning);
        }

        /// <summary>
        /// Removes a monitor from the Collector
        /// </summary>
        /// <param name="resourceMonitor">The monitor.</param>
        public void RemoveMonitor(IResourceMonitor resourceMonitor)
        {
            ResourceCurrentState resourceState;
            if (_resourceStates.TryRemove(resourceMonitor, out resourceState))
            {
                if (IsRunning) // No need to Stop it if we are not running
                    resourceState.Stop();

                _logger.LogTrace("Removed Resource Monitor {Monitor}. Collector running state: {IsCollectorRunning}, Resource running state: {IsResourceRunning}",
                    resourceMonitor, IsRunning, resourceMonitor.IsRunning);
            }
            else
            {
                _logger.LogWarning("Tried to remove a Resource Monitor with no Resource State being collected.");
            }
        }

        protected override void DoStart()
        {
            foreach (var resourceState in _resourceStates)
            {
                resourceState.Value.Start();
            }
        }

        protected override void DoStop()
        {
            foreach (var resourceMonitor in _resourceStates)
            {
                resourceMonitor.Value.Stop();
            }
        }

        /// <summary>
        /// Gets the latests Resource States available
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IResourceCurrentState> GetStates()
        {
            return _resourceStates.Values;
        }

        /// <summary>
        /// Private class to encapsulate Starting/subscribing and Stopping/unsubscribing
        /// </summary>
        /// <seealso cref="IResourceCurrentState" />
        [DebuggerDisplay("Up: {IsUp} - Monitor: {ResourceMonitor.ResourceName}")]
        private sealed class ResourceCurrentState : IResourceCurrentState
        {
            private readonly EventHandler<ResourceMonitorEventArgs> _handler;
            private readonly BoundedQueue<ResourceMonitorEventArgs> _boundedQueue;

            /// <summary>
            /// Uses the latest Event as an indication whether this Resource is Up or not
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance is up; otherwise, <c>false</c>.
            /// </value>
            public bool IsUp
            {
                get
                {
                    var lastEvent = MonitorEvents.LastOrDefault();
                    return lastEvent != null && lastEvent.IsUp;
                }
            }

            /// <summary>
            /// Gets the Monitor Events for this resource
            /// </summary>
            /// <value>
            /// The monitor events.
            /// </value>
            ///
            public IEnumerable<IResourceMonitorEvent> MonitorEvents => _boundedQueue;
            /// <summary>
            /// Gets the resource monitor which does the actual check and reports state
            /// </summary>
            /// <value>
            /// The resource monitor.
            /// </value>
            public IResourceMonitor ResourceMonitor { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="ResourceCurrentState"/> class.
            /// </summary>
            /// <param name="resourceMonitor">The resource monitor.</param>
            /// <param name="maxStatePerResource">The maximum state per resource.</param>
            /// <exception cref="ArgumentNullException"></exception>
            public ResourceCurrentState(IResourceMonitor resourceMonitor, int maxStatePerResource)
            {
                var boundedQueue = new BoundedQueue<ResourceMonitorEventArgs>(maxStatePerResource);
                if (resourceMonitor == null) throw new ArgumentNullException(nameof(resourceMonitor));
                ResourceMonitor = resourceMonitor;
                _boundedQueue = boundedQueue;

                _handler = (sender, @event) =>
                {
                    _boundedQueue.Enqueue(@event);
                };
            }

            public void Start()
            {
                ResourceMonitor.MonitoringEvent += _handler;
                ResourceMonitor.Start();
            }

            public void Stop()
            {
                ResourceMonitor.Stop();
                ResourceMonitor.MonitoringEvent -= _handler;
            }
        }
    }
}