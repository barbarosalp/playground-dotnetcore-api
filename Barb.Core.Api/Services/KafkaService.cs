using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Barb.Core.Api.Configuration;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Barb.Core.Api.Services
{
    public class KafkaService : IKafkaService
    {
        private readonly ILogger<KafkaService> _logger;
        private readonly IOptions<ApplicationConfiguration> _config;
        private readonly IProducer<string, string> _producer;
        private readonly ConcurrentDictionary<string, IConsumer<string, string>> _consumers;

        public KafkaService(ILogger<KafkaService> logger, IOptions<ApplicationConfiguration> config)
        {
            _logger = logger;
            _config = config;

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = $"{config.Value.KafkaHost}:{config.Value.KafkaPort}"
            };

            _producer = new ProducerBuilder<string, string>(producerConfig).Build();
            _consumers = new ConcurrentDictionary<string, IConsumer<string, string>>();
        }

        public void SendMessage(string topicName, string message)
        {
            _producer.Produce(topicName, new Message<string, string>() {Value = message},
                report =>
                {
                    if (report.Error.IsError)
                    {
                        _logger.LogError(report.Error.Reason);
                    }
                });
        }

        public void Consume(
            string topicName,
            string groupId,
            CancellationToken token,
            Action<ConsumeResult<string, string>> onMessage)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = $"{_config.Value.KafkaHost}:{_config.Value.KafkaPort}",
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            var consumer = _consumers.AddOrUpdate(topicName,
                s =>
                {
                    var consumerToInit = new ConsumerBuilder<string, string>(consumerConfig).Build();
                    consumerToInit.Subscribe(topicName);
                    return consumerToInit;
                }, (s, oldConsumer) =>
                {
                    oldConsumer.Close();

                    var consumerToBeUpdated = new ConsumerBuilder<string, string>(consumerConfig).Build();
                    consumerToBeUpdated.Subscribe(topicName);
                    return consumerToBeUpdated;
                }
            );

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var message = consumer.Consume(token);
                    onMessage.Invoke(message);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }
            }

            consumer.Close();
            _consumers.TryRemove(topicName, out consumer);
        }
    }
}