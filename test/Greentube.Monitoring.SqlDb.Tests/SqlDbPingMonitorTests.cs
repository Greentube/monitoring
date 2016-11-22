using System;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Greentube.Monitoring.SqlDb.Tests
{
    public sealed class SqlDbPingMonitorTests
    {
        private sealed class Fixture
        {
            public IDbConnectionProvider DatabaseProvider { get; set; } = Substitute.For<IDbConnectionProvider>();
            public ILogger<SqlDbPingMonitor> Logger { private get; set; } = Substitute.For<ILogger<SqlDbPingMonitor>>();
            public ResourceMonitorConfiguration ResourceMonitorConfiguration { private get; set; } = new ResourceMonitorConfiguration(
                true,
                TimeSpan.FromMilliseconds(0),
                TimeSpan.FromMilliseconds(0),
                TimeSpan.FromSeconds(2));

            private bool IsCritical { get; } = true;
            public string ResourceName { private get; set; } = "SQL Database 123";

            public SqlDbPingMonitor GetSut()
            {
                return new SqlDbPingMonitor(
                    DatabaseProvider,
                    Logger,
                    ResourceMonitorConfiguration,
                    ResourceName,
                    IsCritical);
            }
        }

        private readonly Fixture _fixture = new Fixture();
        

        [Fact]
        public void Constructor_NullMongoDatabase_ThrowsNullArgument()
        {
            _fixture.DatabaseProvider = null;
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
