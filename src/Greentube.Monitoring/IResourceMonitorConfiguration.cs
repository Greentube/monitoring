using System;

namespace Greentube.Monitoring
{
    /// <summary>
    /// Resource Monitor configuration
    /// </summary>
    /// <seealso cref="IResourceMonitor"/>
    public interface IResourceMonitorConfiguration
    {
        /// <summary>
        /// Gets or sets the interval when Resource is Down
        /// </summary>
        /// <value>
        /// The interval when down.
        /// </value>
        TimeSpan IntervalWhenDown { get; }

        /// <summary>
        /// Gets or sets the interval when Resource is up.
        /// </summary>
        /// <value>
        /// The interval when up.
        /// </value>
        TimeSpan IntervalWhenUp { get; }

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        /// <value>
        /// The timeout.
        /// </value>
        TimeSpan Timeout { get; }

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
        bool RunFirstCheckSynchronously { get; }
    }
}
