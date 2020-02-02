using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Barb.Core.Api.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _database;

        public RedisService(ILogger<RedisService> logger, IConfiguration config)
        {
            var host = config["RedisHost"];
            var port = Convert.ToInt32(config["RedisPort"]);
            try
            {
                var configString = $"{host}:{port},abortConnect=false";
                var redis = ConnectionMultiplexer.Connect(configString);
                _database = redis.GetDatabase(1);
            }
            catch (RedisConnectionException err)
            {
                logger.LogError(err.ToString());
                throw err;
            }

            logger.LogDebug("Connected to Redis");
        }
        
        public void Execute(Action<IDatabase> action)
        {
            action(_database);
        }

        public async Task ExecuteAsync(Func<IDatabase,Task> action)
        {
            await action(_database);
        }
    }
}