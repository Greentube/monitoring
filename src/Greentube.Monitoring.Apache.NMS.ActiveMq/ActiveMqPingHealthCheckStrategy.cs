using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Apache.NMS;
using Apache.NMS.ActiveMQ.Commands;

namespace Greentube.Monitoring.Apache.NMS.ActiveMq
{
    /// <summary>
    /// Health check of ActiveMQ by creating connection and sending empty message to temporary queue
    /// </summary>
    public class ActiveMqPingHealthCheckStrategy : IHealthCheckStrategy
    {
        private readonly IConnectionFactory connectionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveMqPingHealthCheckStrategy"/> class.
        /// </summary>
        /// <param name="connectionFactory">Connection factory object</param>
        public ActiveMqPingHealthCheckStrategy(IConnectionFactory connectionFactory)
        {
            if (connectionFactory == null) throw new ArgumentNullException(nameof(connectionFactory));
            this.connectionFactory = connectionFactory;
        }


        /// <inheritdoc />
        public Task<bool> Check(CancellationToken token)
        {
            return Task.Run(() =>
            {
                using (var connection = connectionFactory.CreateConnection())
                {
                    var session = connection.CreateSession();
                    var queue = session.CreateTemporaryQueue();
                    var producer = session.CreateProducer(queue);

                    IMessage message = new ActiveMQMessage();
                    message.NMSMessageId = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
                    message.NMSPriority = MsgPriority.AboveLow;
                    message.NMSTimeToLive = TimeSpan.FromSeconds(5);
                    message.NMSDeliveryMode = MsgDeliveryMode.NonPersistent;

                    producer.Send(message);
                }
                return true;
            }, token);
        }
    }
}