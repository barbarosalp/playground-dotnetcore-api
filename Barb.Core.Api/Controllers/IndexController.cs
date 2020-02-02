using System;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Barb.Core.Api.Configuration;
using Barb.Core.Api.Models;
using Barb.Core.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Barb.Core.Api.Controllers
{
    [ApiController]
    [Route("/api/whiskies")]
    public class IndexController : ControllerBase
    {
        private readonly ILogger<IndexController> _logger;
        private readonly IOptionsMonitor<ApplicationConfiguration> _config;
        private readonly IRedisService _redis;
        private readonly IKafkaService _kafka;
        private readonly Random _random = new Random();

        public IndexController(
            ILogger<IndexController> logger,
            IOptionsMonitor<ApplicationConfiguration> config,
            IRedisService redis,
            IKafkaService kafka
        )
        {
            _logger = logger;
            _config = config;
            _redis = redis;
            _kafka = kafka;
        }

        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        public ActionResult<ApplicationConfiguration> GetIndex()
        {
            return _config.CurrentValue;
        }

        [HttpGet("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<Whisky>> GetIndex(int id)
        {
            Whisky item = null;
            await _redis.ExecuteAsync(async redis =>
            {
                var response = await redis.StringGetAsync($"whiskey:{id}");
                item = JsonSerializer.Deserialize<Whisky>(response);
                
            });

            return Ok(item);
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<Whisky> Create(Whisky whisky)
        {
            whisky.Id = _random.Next(100000000);

            _kafka.SendMessage("messages", JsonSerializer.Serialize(whisky));

            return new StatusCodeResult(201);
        }
    }
}