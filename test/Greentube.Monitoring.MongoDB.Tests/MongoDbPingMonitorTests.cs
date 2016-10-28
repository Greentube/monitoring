using System;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NSubstitute;
using Xunit;

namespace Greentube.Monitoring.MongoDB.Tests
{
    public sealed class MongoDbPingMonitorTests
    {
        private sealed class Fixture
        {
            public IMongoDatabase MongoDatabase { get; set; } = Substitute.For<IMongoDatabase>();
            public ILogger<MongoDbPingMonitor> Logger { private get; set; } = Substitute.For<ILogger<MongoDbPingMonitor>>();
            public ResourceMonitorConfiguration ResourceMonitorConfiguration { private get; set; } = new ResourceMonitorConfiguration(
                true,
                TimeSpan.FromMilliseconds(0),
                TimeSpan.FromMilliseconds(0),
                TimeSpan.FromSeconds(2));

            private bool IsCritical { get; } = true;
            public string ResourceName { private get; set; } = "MongoDB 123";

            public MongoDbPingMonitor GetSut()
            {
                return new MongoDbPingMonitor(
                    MongoDatabase,
                    Logger,
                    ResourceMonitorConfiguration,
                    ResourceName,
                    IsCritical);
            }
        }

        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void ResourceName_IncludesEndPoints()
        {
            var expected = new[]
            {
                new MongoServerAddress("1.1.1.1", 1),
                new MongoServerAddress("2.2.2.2", 2)
            };

            var client = Substitute.For<IMongoClient>();
            var settings = new MongoClientSettings { Servers = expected };
            client.Settings.Returns(settings);
            _fixture.MongoDatabase.Client.Returns(client);
            _fixture.ResourceName = null;

            var target = _fixture.GetSut();

            foreach (var address in expected)
            {
                Assert.Contains($"{address.Host}:{address.Port}", target.ResourceName);
            }
        }

        [Fact]
        public void Constructor_NullMongoDatabase_ThrowsNullArgument()
        {
            _fixture.MongoDatabase = null;
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
