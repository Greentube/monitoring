using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using NSubstitute;
using Xunit;
using NSubstitute.ExceptionExtensions;

namespace Greentube.Monitoring.MongoDB.Tests
{
    public class MongoDbPingHealthCheckStrategyTests
    {
        private class Fixture
        {
            public IMongoDatabase MongoDatabase { get; set; } = Substitute.For<IMongoDatabase>();

            public MongoDbPingHealthCheckStrategy GetSut()
            {
                return new MongoDbPingHealthCheckStrategy(MongoDatabase);
            }
        }

        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public async Task Check_PingCommandSuccess_ReturnsTrue()
        {
            var target = _fixture.GetSut();
            var actual = await target.Check(CancellationToken.None);
            Assert.True(actual);
        }

        [Fact]
        public async Task Check_DatabaseThrows_ExceptionBubbles()
        {
            _fixture.MongoDatabase.RunCommandAsync(Arg.Any<Command<BsonDocument>>())
                .Throws<ArithmeticException>();

            var target = _fixture.GetSut();

            await Assert.ThrowsAsync<ArithmeticException>(() => target.Check(CancellationToken.None));
        }

        [Fact]
        public void Constructor_NullMongoDatabase_ThrowsNullArgument()
        {
            _fixture.MongoDatabase = null;
            Assert.Throws<ArgumentNullException>(() => _fixture.GetSut());
        }
    }
}
