using System;
using System.Net;
using Microsoft.Extensions.Logging;
using NSubstitute;
using StackExchange.Redis;
using Xunit;

namespace Greentube.Monitoring.Redis.Tests
{
    public sealed class RedisPingMonitorTests
    {
        private sealed class Fixture
        {
            public ResourceMonitorConfiguration ResourceMonitorConfiguration { private get; set; } = new ResourceMonitorConfiguration(true);

            public ILogger<RedisPingMonitor> Logger { private get; set; } = Substitute.For<ILogger<RedisPingMonitor>>();

            public IConnectionMultiplexer ConnectionMultiplexer { get; set; } = Substitute.For<IConnectionMultiplexer>();

            private bool IsCritical { get; } = false;

            public Fixture()
            {
                var options = new ConfigurationOptions { EndPoints = { "1.2.3.4" } };
                ConnectionMultiplexer.Configuration.Returns(options.ToString());
            }

            public RedisPingMonitor GetSut()
            {
                return new RedisPingMonitor(
                    IsCritical,
                    ConnectionMultiplexer,
                    Logger,
                    ResourceMonitorConfiguration);
            }
        }

        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void ResourceName_IncludesEndPoints()
        {
            var loopback = IPAddress.Loopback.ToString();
            var broadcast = IPAddress.Broadcast.ToString();
            var options = new ConfigurationOptions { EndPoints = { loopback, broadcast } };

            _fixture.ConnectionMultiplexer.Configuration.Returns(options.ToString());

            var target = _fixture.GetSut();

            Assert.Contains(loopback, target.ResourceName);
            Assert.Contains(broadcast, target.ResourceName);
        }

        [Fact]
        public void Constructor_NullConnectionMultiplexer_ThrowsNullArgument()
        {
            _fixture.ConnectionMultiplexer = null;
            Assert.Throws<ArgumentNullException>(() => _fixture.GetSut());
        }

        [Fact]
        public void Constructor_NullLogger_ThrowsNullArgument()
        {
            _fixture.Logger = null;
            Assert.Throws<ArgumentNullException>(() => _fixture.GetSut());
        }

        [Fact]
        public void Constructor_NullConfiguration_ThrowsNullArgument()
        {
            _fixture.ResourceMonitorConfiguration = null;
            Assert.Throws<ArgumentNullException>(() => _fixture.GetSut());
        }
    }
}
