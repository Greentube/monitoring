using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Greentube.Monitoring.Threading
{
    /// <summary>
    /// Queue that drops items automatically when limit is reached
    /// </summary>
    /// <remarks>
    /// Allows you insert items indefinitely without running out of memory
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Collections.Generic.IEnumerable{T}" />
    internal sealed class BoundedQueue<T> : IEnumerable<T>
    {
        private readonly object _dequeueLock = new object();
        private readonly ConcurrentQueue<T> _concurrentQueue = new ConcurrentQueue<T>();

        private readonly int _maxSize;

        public int Count => _concurrentQueue.Count;

        public BoundedQueue(int maxSize)
        {
            if (maxSize <= 0) throw new ArgumentOutOfRangeException(nameof(maxSize));
            _maxSize = maxSize;
        }

        public void Enqueue(T obj)
        {
            _concurrentQueue.Enqueue(obj);

            if (_concurrentQueue.Count <= _maxSize)
                return;

            lock (_dequeueLock)
            {
                while (_concurrentQueue.Count > _maxSize)
                {
                    T outObj;
                    _concurrentQueue.TryDequeue(out outObj);
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _concurrentQueue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
