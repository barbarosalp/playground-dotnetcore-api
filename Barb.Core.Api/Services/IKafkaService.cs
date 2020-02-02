using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Barb.Core.Api.Services
{
    public interface IKafkaService
    {
        void SendMessage(string topicName, string message);

        void Consume(
            string topicName,
            string groupId,
            CancellationToken token,
            Action<ConsumeResult<string, string>> onMessage
        );
    }
}