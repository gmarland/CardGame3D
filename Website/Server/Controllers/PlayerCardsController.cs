using CardGame.Models;
using Game.DA;
using Game.DB;
using Game.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Website.Server.Controllers
{
    [ApiController, Route("[controller]")]
    public class PlayerCardsController : ControllerBase
    {
        private SystemLogging _sytemLogging;

        private readonly string _dbConnectionString;

        public PlayerCardsController(IConfiguration configuration, ILogger<PlayerCardsController> logger)
        {
            _dbConnectionString = configuration.GetConnectionString("dbConnectionString");

            _sytemLogging = new SystemLogging(logger);
        }

        [HttpGet, Route("/PlayerCards")]
        public async Task<IList<PlayerCard>> GetAll()
        {
            string playerId = "8934";

            CardDataAccess cardDataAccess = new CardDataAccess(_sytemLogging, _dbConnectionString);

            List<PlayerCard> playerCards = new List<PlayerCard>();

            foreach (CardDB card in await cardDataAccess.GetByPlayer(playerId))
            {
                playerCards.Add(new PlayerCard()
                {
                    ID = card.ExternalId,
                    Title = card.Title,
                    Description = card.Description
                });
            }

            return playerCards;
        }
    }
}
