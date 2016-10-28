using System;

namespace Greentube.Monitoring
{
    /// <summary>
    /// Configuration for the Resource Monitor
    /// </summary>
    public class ResourceMonitorConfiguration : IResourceMonitorConfiguration
    {
        /// <summary>
        /// Gets or sets the interval when Resource is Down
        /// </summary>
        /// <value>
        /// The interval when down.
        /// </value>
        public TimeSpan IntervalWhenDown { get; }

        /// <summary>
        /// Gets or sets the interval when Resource is up.
        /// </summary>
        /// <value>
        /// The interval when up.
        /// </value>
        public TimeSpan IntervalWhenUp { get; }

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        /// <value>
        /// The timeout.
        /// </value>
        public TimeSpan Timeout { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [run first check synchronously].
        /// </summary>
        /// <remarks>
        /// When running integration tests, waiting for Timers to trigger the Monitor delays the tests
        /// This flag instructs the Monitors to run the first check synchronously (Starting the Monitor becomes a blocking call)
        /// </remarks>
        /// <value>
        /// <c>true</c> if [run first check synchronously]; otherwise, <c>false</c>.
        /// </value>
        public bool RunFirstCheckSynchronously { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceMonitorConfiguration"/> class.
        /// </summary>
        /// <param name="runFirstCheckSynchronously">if set to <c>true</c> [run first check synchronously].</param>
        /// <param name="intervalWhenDown">The interval when down. Default: 10 seconds.</param>
        /// <param name="intervalWhenUp">The interval when up. Default: 10 seconds.</param>
        /// <param name="timeout">The timeout. Default: Same as intervalWhenDown.</param>
        public ResourceMonitorConfiguration(
            bool runFirstCheckSynchronously,
            TimeSpan? intervalWhenDown = null,
            TimeSpan? intervalWhenUp = null,
            TimeSpan? timeout = null)
        {
            RunFirstCheckSynchronously = runFirstCheckSynchronously;
            IntervalWhenDown = intervalWhenDown ?? TimeSpan.FromSeconds(10);
            IntervalWhenUp = intervalWhenUp ?? TimeSpan.FromSeconds(20);
            Timeout = timeout ?? IntervalWhenDown;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"IntervalWhenDown: {IntervalWhenDown}, IntervalWhenUp: {IntervalWhenUp}, Timeout: {Timeout}";
        }
    }
}
