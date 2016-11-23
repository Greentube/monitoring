using System;

namespace Greentube.Monitoring.Apache.NMS.ActiveMq
{
    /// <summary>
    /// Abstraction that holdes configuration for ActiveMQ monitoring connection
    /// </summary>
    public interface IActiveMqMonitoringConfig
    {
        /// <summary>
        /// represents valid ActiveMQ connection url
        /// </summary>
        Uri Uri { get; }
        /// <summary>
        /// holdes username used to establish ActiveMQ connection
        /// </summary>
        string User { get; }
        /// <summary>
        /// holdes password used to establish ActiveMQ connection
        /// </summary>
        string Password { get; }
    }
}