using System;
using System.Threading;
using System.Threading.Tasks;
using Apache.NMS;
using Apache.NMS.ActiveMQ.Commands;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Greentube.Monitoring.Apache.NMS.ActiveMq.Tests
{
    public class ActiveMqPingHealthCheckStrategyTests
    {
        private class Fixture
        {
            public IConnectionFactory ConnectionFactory { get; set; } = Substitute.For<IConnectionFactory>();

            public ActiveMqPingHealthCheckStrategy GetSut()
            {
                return new ActiveMqPingHealthCheckStrategy(ConnectionFactory);
            }
        }

        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public async Task Check_CreateConnectionThrows_ExceptionBubbles()
        {
            _fixture.ConnectionFactory.CreateConnection().Throws(new ArithmeticException());

            var target = _fixture.GetSut();
            await Assert.ThrowsAsync<ArithmeticException>(() => target.Check(CancellationToken.None));
        }

        [Fact]
        public async Task Check_CreateSessionThrows_ExceptionBubbles()
        {
            var connection = Substitute.For<IConnection>();

            _fixture.ConnectionFactory.CreateConnection().Returns(connection);
            connection.CreateSession().ThrowsForAnyArgs<ArithmeticException>();

            var target = _fixture.GetSut();
            await Assert.ThrowsAsync<ArithmeticException>(() => target.Check(CancellationToken.None));
        }

        [Fact]
        public async Task Check_CreateProducerThrows_ExceptionBubbles()
        {
            var connection = Substitute.For<IConnection>();
            var session = Substitute.For<ISession>();

            _fixture.ConnectionFactory.CreateConnection().Returns(connection);
            connection.CreateSession().Returns(session);
            session.CreateProducer(Arg.Any<IDestination>()).Throws<ArithmeticException>();

            var target = _fixture.GetSut();
            await Assert.ThrowsAsync<ArithmeticException>(() => target.Check(CancellationToken.None));
        }

        [Fact]
        public async Task Check_SendMessageThrows_ExceptionBubbles()
        {
            var connection = Substitute.For<IConnection>();
            var session = Substitute.For<ISession>();
            var producer = Substitute.For<IMessageProducer>();

            _fixture.ConnectionFactory.CreateConnection().Returns(connection);
            connection.CreateSession().Returns(session);
            session.CreateProducer(Arg.Any<IDestination>()).Returns(producer);
            producer.When(p => p.Send(Arg.Any<ActiveMQMessage>())).Do(p => { throw new ArithmeticException(); });

            var target = _fixture.GetSut();
            await Assert.ThrowsAsync<ArithmeticException>(() => target.Check(CancellationToken.None));
        }

        [Fact]
        public async Task Check_SendMessage_ReturnsTrue()
        {
            var connection = Substitute.For<IConnection>();
            var session = Substitute.For<ISession>();
            var producer = Substitute.For<IMessageProducer>();

            _fixture.ConnectionFactory.CreateConnection().Returns(connection);
            connection.CreateSession().Returns(session);
            session.CreateProducer(Arg.Any<IDestination>()).Returns(producer);

            var target = _fixture.GetSut();
            var result = await target.Check(CancellationToken.None);

            producer.Received(1).Send(Arg.Any<ActiveMQMessage>());
            Assert.True(result);
        }

        [Fact]
        public void Constructor_NullConnectionFactory_ThrowsNullArgument()
        {
            _fixture.ConnectionFactory = null;
            Assert.Throws<ArgumentNullException>(() => _fixture.GetSut());
        }
    }
}
