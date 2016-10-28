using System.Linq;
using Greentube.Monitoring.Threading;
using Xunit;

namespace Greentube.Monitoring.Tests
{
    public sealed class BoundedQueueTests
    {
        [Fact]
        public void Enqueue_FullQueue_RemovesFirstItem()
        {
            const int maxSize = 1;
            var first = new object();
            var second = new object();

            var target = new BoundedQueue<object>(maxSize);

            target.Enqueue(first);
            target.Enqueue(second);

            Assert.Equal(maxSize, target.Count);
            Assert.Equal(second, target.Single());
        }

        [Fact]
        public void Enqueue_EnumeratesItems()
        {
            var numbers = Enumerable.Range(1, 10).ToList();

            var maxSize = numbers.Count / 2;
            var target = new BoundedQueue<int>(maxSize);

            for (int i = 0; i < numbers.Count; i++)
            {
                target.Enqueue(numbers[i]);

                // Exact number of enqueued or maxSize
                var expected = i + 1 <= maxSize ? i + 1 : maxSize;
                Assert.Equal(expected, target.Count);
            }
        }
    }
}
