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
    public class PlayerDeckController : ControllerBase
    {
        string playerId = "8934";

        private SystemLogging _systemLogging;

        private readonly string _dbConnectionString;

        public PlayerDeckController(IConfiguration configuration, ILogger<PlayerCardsController> logger)
        {
            _dbConnectionString = configuration.GetConnectionString("dbConnectionString");

            _systemLogging = new SystemLogging(logger);
        }

        [HttpGet, Route("/PlayerDeck/{gameScenarioId}")]
        public async Task<IList<string>> GetAll(string gameScenarioId)
        {
            DeckService deckService = new DeckService(_systemLogging, _dbConnectionString);

            return await deckService.GetPlayerDeck(playerId, gameScenarioId);
        }

        [HttpPut, Route("/PlayerDeck/{gameScenarioId}")]
        public async Task Update(string gameScenarioId, IList<string> cardIds)
        {
            DeckService deckService = new DeckService(_systemLogging, _dbConnectionString);

            await deckService.UpdatePlayerDeck(playerId, gameScenarioId, cardIds);
        }
    }
}
