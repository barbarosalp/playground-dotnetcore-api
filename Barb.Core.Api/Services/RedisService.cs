using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Barb.Core.Api.Services
{
    public class RedisService
    {
        private readonly ILogger<RedisService> _logger;
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisService(ILogger<RedisService> logger, IConfiguration config)
        {
            _logger = logger;

            var host = config["RedisHost"];
            var port = Convert.ToInt32(config["RedisPort"]);
            try
            {
                var configString = $"{host}:{port},connectRetry=5,connectTimeout=10000";
                _redis = ConnectionMultiplexer.Connect(configString);
                _database = _redis.GetDatabase(1);
            }
            catch (RedisConnectionException err)
            {
                _logger.LogError(err.ToString());
                throw err;
            }

            _logger.LogDebug("Connected to Redis");
        }

        public void Execute(Action<IDatabase> action)
        {
            action.Invoke(_database);
        }
    }
}