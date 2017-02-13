namespace Greentube.Monitoring
{
    /// <summary>
    /// Version information of the running service
    /// </summary>
    public interface IVersionService
    {
        /// <summary>
        /// Gets the version information of the running process.
        /// </summary>
        /// <returns></returns>
        VersionInformation GetVersionInformation();
    }
}