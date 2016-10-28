using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using StackExchange.Redis;
using Xunit;
using NSubstitute.ExceptionExtensions;

namespace Greentube.Monitoring.Redis.Tests
{
    public class RedisPingHealthCheckStrategyTests
    {
        private class Fixture
        {
            public IConnectionMultiplexer Multiplexer { get; set; } = Substitute.For<IConnectionMultiplexer>();

            public RedisPingHealthCheckStrategy GetSut()
            {
                return new RedisPingHealthCheckStrategy(Multiplexer);
            }
        }

        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public async Task Check_IsConnectedTrue_ReturnsTrue()
        {
            _fixture.Multiplexer.IsConnected.Returns(true);

            var target = _fixture.GetSut();
            var actual = await target.Check(CancellationToken.None);
            Assert.True(actual);
        }

        [Fact]
        public async Task Check_IsConnectedFalsePingSuccessful_ReturnsTrue()
        {
            _fixture.Multiplexer.IsConnected.Returns(false);

            _fixture.Multiplexer.GetDatabase(0)
                .PingAsync(Arg.Any<CommandFlags>())
                .Returns(TimeSpan.FromMilliseconds(1));

            var target = _fixture.GetSut();
            var actual = await target.Check(CancellationToken.None);
            Assert.True(actual);
        }

        [Fact]
        public async Task Check_IsConnectedFalsePingThrows_ExceptionBubbles()
        {
            _fixture.Multiplexer.IsConnected.Returns(false);

            _fixture.Multiplexer
                .GetDatabase(0)
                .PingAsync(Arg.Any<CommandFlags>())
                .Throws(new ArithmeticException());

            var target = _fixture.GetSut();

            await Assert.ThrowsAsync<ArithmeticException>(() => target.Check(CancellationToken.None));
        }

        [Fact]
        public void Constructor_NullConnectionMultiplexer_ThrowsNullArgument()
        {
            _fixture.Multiplexer = null;
            Assert.Throws<ArgumentNullException>(() => _fixture.GetSut());
        }
    }
}
