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
    }
}
