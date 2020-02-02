using System.Threading;
using System.Threading.Tasks;
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
                _logger.LogDebug($" GracePeriod background task is stopping.")); 
            
            
            _kafkaService.Consume("messages", "group-messages", stoppingToken,
                message =>
                {
                    _logger.LogInformation($"Key: {message.Key} => {message.Value}");
                });
        
        
            _logger.LogDebug($"Background service is stopping.");
            
        }
        
    }
}