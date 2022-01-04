using CardGame.Models;
using Game.DB;
using Game.Logging;
using Game.SL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Website.Server.Controllers
{
    [ApiController, Route("[controller]")]
    public class ActsController : ControllerBase
    {
        private SystemLogging _sytemLogging;

        private readonly string _dbConnectionString;

        public ActsController(IConfiguration configuration, ILogger<LocationController> logger)
        {
            _dbConnectionString = configuration.GetConnectionString("dbConnectionString");

            _sytemLogging = new SystemLogging(logger);
        }

        [HttpGet, Route("/Acts/{gameScenarioId}")]
        public async Task<IList<Act>> GetAllActs(string gameScenarioId)
        {
            ActService actService = new ActService(_sytemLogging, _dbConnectionString);

            List<Act> acts = new List<Act>();

            foreach (ActDB act in await actService.GetAllGameScenarioActs(gameScenarioId))
            {
                acts.Add(new Act()
                {
                    ID = act.ExternalId,
                    Title = act.Title,
                    Description = act.Description,
                    Completion = act.Completion,
                    RequiredTokens = act.RequiredTokens,
                    SequenceNo = act.SequenceNo
                });
            }

            return acts;
        }
    }
}
