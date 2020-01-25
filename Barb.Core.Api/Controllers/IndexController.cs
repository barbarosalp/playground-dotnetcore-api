using System.Net.Mime;
using Barb.Core.Api.Configuration;
using Barb.Core.Api.Models;
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
        private readonly Whisky _whisky = new Whisky("Burak", "Kusadasi");

        public IndexController(ILogger<IndexController> logger, IOptionsMonitor<ApplicationConfiguration> config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        public ActionResult<ApplicationConfiguration> GetIndex()
        {
            return _config.CurrentValue;
        }
    }
}