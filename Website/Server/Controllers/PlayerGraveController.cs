using CardGame.Models;
using Game.DA;
using Game.DB;
using Game.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Server.Controllers
{
    [ApiController, Route("[controller]")]
    public class PlayerGraveController : ControllerBase
    {
        private SystemLogging _sytemLogging;

    private readonly string _dbConnectionString;

    public PlayerGraveController(IConfiguration configuration, ILogger<PlayerGraveController> logger)
    {
        _dbConnectionString = configuration.GetConnectionString("dbConnectionString");

        _sytemLogging = new SystemLogging(logger);
    }

        [HttpGet, Route("/PlayerGrave/{gameScenarioId}")]
        public async Task<IList<string>> GetAll(string gameScenarioId)
        {
            string playerId = "8934";

            CardDataAccess cardDataAccess = new CardDataAccess(_sytemLogging, _dbConnectionString);
            PlayerGraveDataAccess playerGraveDataAccess = new PlayerGraveDataAccess(_sytemLogging, _dbConnectionString);

            IList<CardDB> allCards = await cardDataAccess.GetByPlayer(playerId);

            List<string> playerHand = new List<string>();

            foreach (PlayerGraveDB graveCard in (await playerGraveDataAccess.GetByPlayerGrave(gameScenarioId, playerId)).OrderBy(h => h.SequenceNo))
            {
                CardDB card = allCards.FirstOrDefault(c => c.Id == graveCard.CardId);

                if (card != null) playerHand.Add(card.ExternalId);
            }

            return playerHand;
        }
    }
}
