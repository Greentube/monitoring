namespace Greentube.Monitoring.InternalResource
{
    /// <summary>
    /// Represents internal resource state
    /// </summary>
    public interface IInternalResourceMonitored
    {
    /// <summary>
    /// Gets a value indicating whether this resource is up.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is up; otherwise, <c>false</c>.
    /// </value>
    bool IsUp { get; }
    }
}