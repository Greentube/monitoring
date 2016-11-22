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
        string Url { get; }
        /// <summary>
        /// holdes username used to establish ActiveMQ connection
        /// </summary>
        string User { get; }
        /// <summary>
        /// holdes password used to establish ActiveMQ connection
        /// </summary>
        string Password { get; }
    }

    /// <inheritdoc cref="IActiveMqMonitoringConfig"/>
    public class ActiveMqMonitoringConfig : IActiveMqMonitoringConfig
    {
        /// <inheritdoc cref="IActiveMqMonitoringConfig.Url"/>
        public string Url { get; set; }
        /// <inheritdoc cref="IActiveMqMonitoringConfig.User"/>
        public string User { get; set; } = "";
        /// <inheritdoc cref="IActiveMqMonitoringConfig.Password"/>
        public string Password { get; set; } = "";
    }
}