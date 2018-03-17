namespace Greentube.Monitoring
{
    internal sealed class ShutdownStatusProvider : IShutdownStatusProvider
    {
        public ShutdownStatusProvider()
        {
            IsShuttingDown = false;
        }

        public bool IsShuttingDown { get; private set; }

        public void Shutdown()
        {
            IsShuttingDown = true;
        }
    }
}