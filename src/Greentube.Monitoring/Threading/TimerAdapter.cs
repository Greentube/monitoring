using System;
using System.Threading;

namespace Greentube.Monitoring.Threading
{
    /// <summary>
    /// Adapts <see cref="ITimer"/> to <see cref="System.Threading.Timer"/>
    /// </summary>
    /// <seealso cref="ITimer" />
    internal sealed class TimerAdapter : ITimer
    {
        private readonly Timer _timer;

        public TimerAdapter(TimerCallback callback)
        {
            _timer = new Timer(callback, null, -1, -1);
        }

        public bool Change(TimeSpan dueTime, TimeSpan period)
        {
            return _timer.Change(dueTime, period);
        }

        public bool Change(int dueTime, int period)
        {
            return _timer.Change(dueTime, period);
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}