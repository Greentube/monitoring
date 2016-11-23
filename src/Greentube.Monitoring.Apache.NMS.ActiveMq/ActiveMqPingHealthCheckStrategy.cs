using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Apache.NMS;
using Apache.NMS.ActiveMQ.Commands;

namespace Greentube.Monitoring.Apache.NMS.ActiveMq
{
    /// <summary>
    /// Health check of ActiveMQ by creating connection and sending empty message to a queue
    /// </summary>
    public class ActiveMqPingHealthCheckStrategy : IHealthCheckStrategy
    {
        private readonly string destinationName = "healthCheckPingQueue";
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
            using (var connection0 = connectionFactory.CreateConnection())
            {
                using (var session = connection0.CreateSession())
                {
                    using (var destination = new ActiveMQQueue(destinationName))
                    {
                        using (var producer = session.CreateProducer(destination))
                        {
                            producer.DeliveryMode = MsgDeliveryMode.NonPersistent;
                            producer.Priority = MsgPriority.AboveLow;

                            var message = new ActiveMQMessage();
                            message.NMSTimeToLive = TimeSpan.FromMilliseconds(1000);
                            producer.Send(message);
                        }
                    }
                }
            }
            return Task.FromResult(true);
        }
    }
}