using Game.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Website.Server.Controllers
{
    [ApiController, Route("[controller]")]
    public class GameController : ControllerBase
    {
        private SystemLogging _sytemLogging;

        private readonly string _dbConnectionString;

        public GameController(IConfiguration configuration, ILogger<LocationController> logger)
        {
            _dbConnectionString = configuration.GetConnectionString("dbConnectionString");

            _sytemLogging = new SystemLogging(logger);
        }
    }
}
