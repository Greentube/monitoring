using System;
using Apache.NMS;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Greentube.Monitoring.Apache.NMS.ActiveMq.Tests
{
    public sealed class ActiveMqPingMonitorTest
    {
        private sealed class Fixture
        {
            public IConnectionFactory ConnectionFactory { get; set; } = Substitute.For<IConnectionFactory>();
            public ILogger<ActiveMqPingMonitor> Logger { private get; set; } = Substitute.For<ILogger<ActiveMqPingMonitor>>();
            public ResourceMonitorConfiguration ResourceMonitorConfiguration { private get; set; } = new ResourceMonitorConfiguration(
                true,
                TimeSpan.FromMilliseconds(0),
                TimeSpan.FromMilliseconds(0),
                TimeSpan.FromSeconds(2));

            private bool IsCritical { get; } = true;
            public string ResourceName { private get; set; } = "ActiveMQ 123";
            public string DestinationQueueName { private get; set; } = "pingQueue";

            public ActiveMqPingMonitor GetSut()
            {
                return new ActiveMqPingMonitor(
                    ResourceName,
                    ConnectionFactory,
                    DestinationQueueName,
                    ResourceMonitorConfiguration,
                    Logger,
                    IsCritical);
            }
        }

        private readonly Fixture _fixture = new Fixture();
        

        [Fact]
        public void Constructor_NullMongoDatabase_ThrowsNullArgument()
        {
            _fixture.ConnectionFactory = null;
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
