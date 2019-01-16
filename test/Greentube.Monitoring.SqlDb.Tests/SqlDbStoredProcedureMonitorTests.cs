using System;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Greentube.Monitoring.SqlDb.Tests
{
    public sealed class SqlDbStoredProcedureMonitorTests
    {
        private sealed class Fixture
        {
            public IDbConnectionProvider ConnectionProvider { get; set; }
            public string Schema { get; set; }
            public string StoredProcedureName { get; set; }
            public ILogger<ResourceMonitor> Logger { get; set; }
            public IResourceMonitorConfiguration ResourceMonitorConfiguration { get; set; }
            public string ResourceName { get; set; }
            public bool IsCritical { get; set; }

            public Fixture()
            {
                ConnectionProvider = Substitute.For<IDbConnectionProvider>();
                Schema = "Schema";
                StoredProcedureName = "SP";
                Logger = Substitute.For<ILogger<ResourceMonitor>>();
                ResourceMonitorConfiguration = Substitute.For<IResourceMonitorConfiguration>();
                ResourceName = "Resource";
                IsCritical = true;
            }

            public SqlDbStoredProcedureMonitor BuildSut() => new SqlDbStoredProcedureMonitor(ConnectionProvider, Schema,
                StoredProcedureName, Logger, ResourceMonitorConfiguration, ResourceName, IsCritical);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Constructor_Ok(bool isCritical)
        {
            var fixture = new Fixture
            {
                IsCritical = isCritical
            };
            var sut = fixture.BuildSut();

            Assert.Equal(fixture.ResourceName, sut.ResourceName);
            Assert.Equal(fixture.ResourceMonitorConfiguration, sut.Configuration);
            Assert.Equal(isCritical, sut.IsCritical); 
        }

        [Fact]
        public void Constructor_NullDbConnectionProvider_Throws()
        {
            var fixture = new Fixture {ConnectionProvider = null};
            Assert.Throws<ArgumentNullException>(() => fixture.BuildSut());
        }

        [Fact]
        public void Constructor_NullSchema_Throws()
        {
            var fixture = new Fixture { Schema = null };
            Assert.Throws<ArgumentNullException>(() => fixture.BuildSut());
        }

        [Fact]
        public void Constructor_NullStoredProcedure_Throws()
        {
            var fixture = new Fixture { StoredProcedureName = null };
            Assert.Throws<ArgumentNullException>(() => fixture.BuildSut());
        }

        [Fact]
        public void Constructor_NullLogger_Throws()
        {
            var fixture = new Fixture { Logger = null };
            Assert.Throws<ArgumentNullException>(() => fixture.BuildSut());
        }

        [Fact]
        public void Constructor_NullConfiguration_Throws()
        {
            var fixture = new Fixture { ResourceMonitorConfiguration = null };
            Assert.Throws<ArgumentNullException>(() => fixture.BuildSut());
        }

        [Fact]
        public void Constructor_NullResourceName_Throws()
        {
            var fixture = new Fixture { ResourceName = null };
            Assert.Throws<ArgumentNullException>(() => fixture.BuildSut());
        }
    }
}
