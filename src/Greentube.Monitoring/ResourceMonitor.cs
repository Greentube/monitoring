using System;
using System.Diagnostics;
using System.Threading;
using Greentube.Monitoring.Threading;
using Microsoft.Extensions.Logging;

namespace Greentube.Monitoring
{
    /// <summary>
    /// Resource Monitor: when started, raises events reporting the status of a resource
    /// </summary>
    /// <seealso cref="IResourceMonitor" />
    /// <seealso cref="IDisposable" />
    public class ResourceMonitor : AbstractStartable, IResourceMonitor, IDisposable
    {
        private readonly object _verificationLock = new object();
        private readonly ITimer _timer;
        private readonly ILogger<ResourceMonitor> _logger;
        private readonly IHealthCheckStrategy _verificationStrategy;

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        [PublicAPI]
        public IResourceMonitorConfiguration Configuration { get; }

        /// <summary>
        /// Gets the name of the resource.
        /// </summary>
        /// <value>
        /// The name of the resource.
        /// </value>
        public string ResourceName { get; }

        /// <summary>
        /// Gets a value indicating whether this resource is critical to the functioning of the system
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is critical; otherwise, <c>false</c>.
        /// </value>
        public bool IsCritical { get; }

        /// <summary>
        /// Occurs when a verification is executed
        /// </summary>
        public event EventHandler<ResourceMonitorEventArgs> MonitoringEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceMonitor"/> class.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="verificationStrategy">The verify strategy.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="isCritical">if set to <c>true</c> [is critical].</param>
        [PublicAPI]
        public ResourceMonitor(
            string resourceName,
            IHealthCheckStrategy verificationStrategy,
            IResourceMonitorConfiguration configuration,
            ILogger<ResourceMonitor> logger,
            bool isCritical = false)
            : this(resourceName,
                    verificationStrategy,
                    configuration,
                    logger,
                    isCritical,
                    new TimerAdapterFactory())
        {
        }

        internal ResourceMonitor(
            string resourceName,
            IHealthCheckStrategy verificationStrategy,
            IResourceMonitorConfiguration configuration,
            ILogger<ResourceMonitor> logger,
            bool isCritical,
            ITimerFactory timerFactory)
        {
            if (resourceName == null) throw new ArgumentNullException(nameof(resourceName));
            if (verificationStrategy == null) throw new ArgumentNullException(nameof(verificationStrategy));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (timerFactory == null) throw new ArgumentNullException(nameof(timerFactory));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            _verificationStrategy = verificationStrategy;
            _logger = logger;
            ResourceName = resourceName;
            Configuration = configuration;
            IsCritical = isCritical;

            _timer = timerFactory.Create(OnTimer);
            if (_timer == null)
                throw new InvalidOperationException("Expected TimerFactory to return a Timer.");
        }

        /// <summary>
        /// Starts to Monitor the resource
        /// </summary>
        protected override void DoStart()
        {
            if (Configuration.StartSynchronously)
            {
                _logger.LogInformation("Starting ResourceMonitor - Executing the first check for: {ResourceName} synchronously", ResourceName);
                Verify();
            }
            else
            {
                var period = Configuration.IntervalWhenUp;
                _logger.LogInformation("Starting ResourceMonitor - Scheduling the first check for: {ResourceName} in {Period}", ResourceName, period);

                _timer.Change(TimeSpan.Zero, period);
            }
        }

        /// <summary>
        /// Stops monitoring the resource
        /// </summary>
        protected override void DoStop()
        {
            _logger.LogInformation("Stopping ResourceMonitor: {ResourceName}", ResourceName);
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void OnTimer(object _)
        {
            // If the timer is faster than the verification, skip the check
            if (Monitor.TryEnter(_verificationLock))
            {
                try
                {
                    Verify();
                }
                finally
                {
                    Monitor.Exit(_verificationLock);
                }
            }
        }

        internal void Verify() // Internal for testability
        {
            var sw = Stopwatch.StartNew();

            var evt = CreateVerificationEvent();
            var logLevelForDown = IsCritical ? LogLevel.Error : LogLevel.Warning;
            var logLevel = evt.IsUp ? LogLevel.Trace : logLevelForDown;
            _logger.Log(logLevel, 0, "{verificationTimeUtc}, {resource}, {critical}, {up}, {ex}", DateTime.UtcNow, ResourceName, IsCritical,
                    evt.IsUp, evt.Exception);
            sw.Stop();
            evt.Latency = sw.Elapsed;

            var period = evt.IsUp
                ? Configuration.IntervalWhenUp
                : Configuration.IntervalWhenDown;

            _timer.Change(period, period);

            OnMonitoringEvent(evt);
        }

        private ResourceMonitorEventArgs CreateVerificationEvent()
        {
            var evt = new ResourceMonitorEventArgs();
            var timedOut = false;
            try
            {
                var source = new CancellationTokenSource(Configuration.Timeout);

                var verificationTask = _verificationStrategy.Check(source.Token);
                timedOut = !verificationTask
                               .Wait(Configuration.Timeout)
                           || source.Token.IsCancellationRequested;

                // It's up if it didn't timeout and the check was OK.
                evt.IsUp = !timedOut && verificationTask.Result;
            }
            catch (Exception ex)
            {
                evt.IsUp = false;
                evt.Exception = ex;
            }
            finally
            {
                if (timedOut) // To avoid throwing from the try block
                    evt.Exception = new TimeoutException();
            }

            return evt;
        }

        private void OnMonitoringEvent(ResourceMonitorEventArgs e)
        {
            try
            {
                MonitoringEvent?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(0, ex, "The event handler has thrown an exception. The Exception will be re-thrown and the Monitoring will stop.");
                throw;
            }
        }

        /// <summary>
        /// Disposes the inner timer, stopping the resource monitoring
        /// </summary>
        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}