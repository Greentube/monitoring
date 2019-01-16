using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;

namespace Greentube.Monitoring.SqlDb.Tests
{
    public sealed class SqlDbStoredProcedureHealthCheckStrategyTests
    {
        [Fact]
        public void Constructor_DbConnectionProviderNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new SqlDbStoredProcedureHealthCheckStrategy(null, "[schema].[storedProcedure]"));
        }

        [Fact]
        public void Constructor_StoredProcedureNameNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new SqlDbStoredProcedureHealthCheckStrategy(Substitute.For<IDbConnectionProvider>(), null));
        }

        [Fact(Skip = "Requires specific database and stored procedure without parameters")]
        public async Task Check_RealStoredProcedureCallSuccess_ReturnsTrue()
        {
            var sut = new SqlDbStoredProcedureHealthCheckStrategy(new DbConnectionProvider(
                "Server=localhost\\CASH;Database=gamebase;Trusted_Connection=True;"),
                "[nrgs].[METADATA_GET]");
            Assert.True(await sut.Check(CancellationToken.None));
        }

        private class Fixture
        {
            public IDbConnectionProvider ConnectionProvider { get; }
            public DbConnection Connection { get; }
            public DbCommand Command { get; }
            public string LastCommandText { get; private set; }
            public CommandType? LastCommandType { get; private set; }

            public Fixture(int? returnValue = null, Exception exception = null)
            {
                ConnectionProvider = Substitute.For<IDbConnectionProvider>();
                Connection = Substitute.For<DbConnection>();
                ConnectionProvider.GetDbConnection().Returns(Connection);
                Command = Substitute.For<DbCommand>();
                Connection.CreateCommand().Returns(Command);
                Command.ExecuteNonQueryAsync().Returns(_ =>
                {
                    if (exception != null)
                        throw exception;
                    return returnValue.GetValueOrDefault(1);
                }).AndDoes(callInfo =>
                {
                    LastCommandText = Command.CommandText;
                    LastCommandType = Command.CommandType;
                });
            }
        }

        [Fact]
        public async Task Check_StoredProcedureCallSuccess_ReturnsTrue()
        {
            // ARRANGE
            var fixture = new Fixture();
            const string storedProcedureName = "schema.spName";
            var sut = new SqlDbStoredProcedureHealthCheckStrategy(fixture.ConnectionProvider, storedProcedureName);

            // ACT
            var result = await sut.Check(CancellationToken.None);

            // ASSERT
            Assert.True(result);
            Assert.Equal(storedProcedureName, fixture.LastCommandText);
            Assert.Equal(CommandType.StoredProcedure, fixture.LastCommandType);
            await fixture.Command.Received(1).ExecuteNonQueryAsync();
            fixture.Command.Received(1).Dispose();
            fixture.Connection.Received(1).Dispose();
        }

        [Fact]
        public async Task Check_StoredProcedureCallThrows_ExceptionBubbles()
        {
            // ARRANGE
            var expectedException = new ArithmeticException();
            var fixture = new Fixture(exception: expectedException);
            const string storedProcedureName = "schema.faultySpName";
            var sut = new SqlDbStoredProcedureHealthCheckStrategy(fixture.ConnectionProvider, storedProcedureName);

            // ACT
            var actualException = await Assert.ThrowsAsync<ArithmeticException>(() => sut.Check(CancellationToken.None));

            // ASSERT
            Assert.Equal(expectedException, actualException);
            Assert.Equal(storedProcedureName, fixture.LastCommandText);
            Assert.Equal(CommandType.StoredProcedure, fixture.LastCommandType);
            await fixture.Command.Received(1).ExecuteNonQueryAsync();
            fixture.Command.Received(1).Dispose();
            fixture.Connection.Received(1).Dispose();
        }
    }
}
