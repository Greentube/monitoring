using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Greentube.Monitoring.SqlDb.Tests
{
    public sealed class SqlDbMonitoringOptionsExtensionsTests
    {
        [Theory]
        [InlineData("Application Name=FooService;Server=localhost\\CASH;Database=Akka;MultipleActiveResultSets=true;Trusted_Connection=True;", "my_schema", "Akka.my_schema")]
        [InlineData("Application Name=Bar;Data Source=dev-db;Initial Catalog=coordinator_db;UID=baz_user;PWD=123456", "SCHEMA", "coordinator_db.SCHEMA")]
        public void DbSchemaResourceName(string dbConnectionString, string schema, string expectedResourceName)
        {
            var resourceName = SqlDbMonitoringOptionsExtensions.DbSchemaResourceName(dbConnectionString, schema);
            Assert.Equal(expectedResourceName, resourceName);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AddSqlDbStoredProcedureMonitor_Ok(bool isCritical)
        {
            // ARRANGE
            var options = new MonitoringOptions();
            var dbConnectionProvider = Substitute.For<IDbConnectionProvider>();
            var configuration = Substitute.For<IResourceMonitorConfiguration>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            var logger = Substitute.For<ILogger<SqlDbStoredProcedureMonitor>>();
            serviceProvider.GetService(typeof(ILogger<SqlDbStoredProcedureMonitor>)).Returns(logger);

            // ACT
            options.AddSqlDbStoredProcedureMonitor(dbConnectionProvider, "schema", "storedProcedureName", "resource", isCritical, configuration);

            // ASSERT
            var factories = options.Factories.ToList();
            Assert.Single(factories);
            var factory = factories.Single();
            var monitor = factory(configuration, serviceProvider);
            Assert.IsType<SqlDbStoredProcedureMonitor>(monitor);
            var sqlMonitor = (SqlDbStoredProcedureMonitor)monitor;
            Assert.Equal("resource", sqlMonitor.ResourceName);
            Assert.Equal(configuration, sqlMonitor.Configuration);
            Assert.Equal(isCritical, sqlMonitor.IsCritical);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AddSqlDbStoredProcedureMonitor2_Ok(bool isCritical)
        {
            // ARRANGE
            var options = new MonitoringOptions();
            var serviceProvider = Substitute.For<IServiceProvider>();
            var logger = Substitute.For<ILogger<SqlDbStoredProcedureMonitor>>();
            serviceProvider.GetService(typeof(ILogger<SqlDbStoredProcedureMonitor>)).Returns(logger);

            // ACT
            options.AddSqlDbStoredProcedureMonitor("Application Name=Bar;Data Source=dev-db;Initial Catalog=db_name;UID=baz_user;PWD=123456", "schema", "storedProcedureName", isCritical: isCritical);

            // ASSERT
            var factories = options.Factories.ToList();
            Assert.Single(factories);
            var factory = factories.Single();
            var monitor = factory(options, serviceProvider);
            Assert.IsType<SqlDbStoredProcedureMonitor>(monitor);
            var sqlMonitor = (SqlDbStoredProcedureMonitor)monitor;
            Assert.Equal("db_name.schema", sqlMonitor.ResourceName);
            Assert.Equal(options, sqlMonitor.Configuration);
            Assert.Equal(isCritical, sqlMonitor.IsCritical);
        }
    }
}
