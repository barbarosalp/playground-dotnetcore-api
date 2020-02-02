using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Barb.Core.Api.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Barb.Core.Api.Services
{
    public class ConsumerService : BackgroundService
    {
        private readonly IKafkaService _kafkaService;
        private readonly IRedisService _redisService;
        private readonly ILogger<ConsumerService> _logger;

        public ConsumerService(IKafkaService kafkaService, IRedisService redisService, ILogger<ConsumerService> logger)
        {
            _kafkaService = kafkaService;
            _redisService = redisService;
            _logger = logger;
            _logger.LogDebug("Background service constructor.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Background service is starting.");

            stoppingToken.Register(() =>
                _logger.LogDebug($" Background task is stopping."));


            await Task.Run(() =>
            {
                _kafkaService.Consume(
                    "messages",
                    "dummy",
                    stoppingToken,
                    message =>
                    {
                        var whisky = JsonSerializer.Deserialize<Whisky>(message.Value);
                        _redisService.Execute(async redis =>
                        {
                            await redis.StringSetAsync($"whiskey:{whisky.Id}", message.Value);
                        });
                    }
                );
            }, stoppingToken);

            _logger.LogDebug($"Background service is stopping.");
        }
    }
}