using System;

namespace Greentube.Monitoring.Threading
{
    public abstract class AbstractStartable : IStartable
    {
        private readonly object _lock = new object();
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">This instance already in running mode. Consider Stopping it first.</exception>
        public void Start()
        {
            lock (_lock)
            {
                if (IsRunning)
                    throw new InvalidOperationException("This instance already in running mode. Consider Stopping it first.");
                DoStart();
                IsRunning = true;
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">This instance is not in running mode. Consider Starting it first.</exception>
        public void Stop()
        {
            lock (_lock)
            {
                if (!IsRunning)
                    throw new InvalidOperationException("This instance is not in running mode. Consider Starting it first.");
                DoStop();
                IsRunning = false;
            }
        }

        protected abstract void DoStart();
        protected abstract void DoStop();
    }
}
