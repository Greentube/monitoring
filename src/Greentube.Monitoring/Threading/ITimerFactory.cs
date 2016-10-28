using System.Threading;

namespace Greentube.Monitoring.Threading
{
    internal interface ITimerFactory
    {
        ITimer Create(TimerCallback callback);
    }
}