using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;
using NSubstitute.ExceptionExtensions;

namespace Greentube.Monitoring.SqlDb.Tests
{
    public class SqlDbPingHealthCheckStrategyTests
    {
        private class Fixture
        {
            public IDbConnectionProvider ConnectionProvider { get; set; } = Substitute.For<IDbConnectionProvider>();

            public SqlDbPingHealthCheckStrategy GetSut()
            {
                return new SqlDbPingHealthCheckStrategy(ConnectionProvider);
            }
        }

        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public async Task Check_PingCommandSuccess_ReturnsTrue()
        {
            var dbCommand = Substitute.For<DbCommand>();
            var connection = Substitute.For<DbConnection>();
            connection.CreateCommand().Returns(dbCommand);
            _fixture.ConnectionProvider.GetDbConnection().Returns(connection);
            var target = _fixture.GetSut();
            var actual = await target.Check(CancellationToken.None);
            Assert.True(actual);
        }

        [Fact]
        public async Task Check_DatabaseThrows_ExceptionBubbles()
        {
            _fixture.ConnectionProvider.GetDbConnection().Throws<ArithmeticException>();

            var target = _fixture.GetSut();

            await Assert.ThrowsAsync<ArithmeticException>(() => target.Check(CancellationToken.None));
        }

        [Fact]
        public void Constructor_NullProvider_ThrowsNullArgument()
        {
            _fixture.ConnectionProvider = null;
            Assert.Throws<ArgumentNullException>(() => _fixture.GetSut());
        }
    }
}
