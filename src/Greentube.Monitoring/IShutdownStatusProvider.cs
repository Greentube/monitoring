namespace Greentube.Monitoring
{
    /// <summary>
    /// Resolve this inteface on application shutdown to inform the loab balancer that it should take down the node.
    /// </summary>
    public interface IShutdownStatusProvider
    {
        /// <summary>
        /// Current status of Shutdown process
        /// </summary>
        bool IsShuttingDown { get; }

        /// <summary>
        /// Inform Resource monitor that application is shutting down
        /// </summary>
        void Shutdown();
    }
}