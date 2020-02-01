using System;
using System.Net.Mime;
using System.Text.Json;
using Barb.Core.Api.Configuration;
using Barb.Core.Api.Models;
using Barb.Core.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Barb.Core.Api.Controllers
{
    [ApiController]
    [Route("/api/whiskies")]
    public class IndexController : ControllerBase
    {
        private readonly ILogger<IndexController> _logger;
        private readonly IOptionsMonitor<ApplicationConfiguration> _config;
        private readonly RedisService _redis;
        private readonly Whisky _whisky = new Whisky("Burak", "Kusadasi");
        private readonly Random _random = new Random();

        public IndexController(
            ILogger<IndexController> logger,
            IOptionsMonitor<ApplicationConfiguration> config,
            RedisService redis)
        {
            _logger = logger;
            _config = config;
            _redis = redis;
        }

        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        public ActionResult<ApplicationConfiguration> GetIndex()
        {
            return _config.CurrentValue;
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<Whisky> Create(Whisky whisky)
        {
            whisky.Id = _random.Next(100000000);
            _redis.Execute(async redis =>
            {
                await redis.StringSetAsync($"whiskey:{whisky.Id}", JsonSerializer.Serialize(whisky));
            });

            return new StatusCodeResult(201);
        }
    }
}