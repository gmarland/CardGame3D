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
    public class PlayerHandController : ControllerBase
    {
        string playerId = "8934";

        private SystemLogging _systemLogging;

        private readonly string _dbConnectionString;

        public PlayerHandController(IConfiguration configuration, ILogger<PlayerCardsController> logger)
        {
            _dbConnectionString = configuration.GetConnectionString("dbConnectionString");

            _systemLogging = new SystemLogging(logger);
        }

        [HttpGet, Route("/PlayerHand/{gameScenarioId}")]
        public async Task<IList<string>> GetAll(string gameScenarioId)
        {
            DeckService deckService = new DeckService(_systemLogging, _dbConnectionString);

            return await deckService.GetPlayerHand(playerId, gameScenarioId);
        }

        [HttpPut, Route("/PlayerHand/{gameScenarioId}")]
        public async Task Update(string gameScenarioId, IList<string> cardIds)
        {
            DeckService deckService = new DeckService(_systemLogging, _dbConnectionString);

            await deckService.UpdatePlayerHand(playerId, gameScenarioId, cardIds);
        }
    }
}