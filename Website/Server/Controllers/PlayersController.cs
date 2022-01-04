using CardGame.Models;
using Game.DB;
using Game.Logging;
using Game.SL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Server.Hubs;

namespace Website.Server.Controllers
{
    [ApiController, Route("[controller]")]
    public class PlayersController : ControllerBase
    {
        string playerId = "8934";

        private SystemLogging _sytemLoggin;

        private readonly string _dbConnectionString;

        private GameHub _gameHub;

        public PlayersController(IConfiguration configuration, ILogger<LocationController> logger, GameHub gameHub)
        {
            _dbConnectionString = configuration.GetConnectionString("dbConnectionString");

            _sytemLoggin = new SystemLogging(logger);

            _gameHub = gameHub;
        }

        [HttpGet, Route("/me/{gameScenarioId}")]
        public async Task<Player> GetMe(string gameScenarioId)
        {
            PlayerService playerService = new PlayerService(_sytemLoggin, _dbConnectionString);

            PlayerDB player = await playerService.GetPlayer(gameScenarioId, playerId);
            PlayerGameScenarioDB playerGameScenarioDB = await playerService.GetGameScenarioPlayer(gameScenarioId, playerId);

            return new Player()
            {
                ID = player.ExternalId,
                Name = player.Name,
                Strength = player.Strength,
                Health = player.Health,
                Sanity = player.Sanity,
                Resources = playerGameScenarioDB.Resources,
                ResourcesPerTurn = player.ResourcesPerTurn
            };
        }

        [HttpGet, Route("/Players/{gameScenarioId}")]
        public async Task<IList<Player>> GetAllPlayers(string gameScenarioId)
        {
            PlayerService playerService = new PlayerService(_sytemLoggin, _dbConnectionString);
            LocationService locationService = new LocationService(_sytemLoggin, _dbConnectionString);

            IList<LocationDB> locationDBs = await locationService.GetAllGameScenarioLocations(gameScenarioId);
            IList<PlayerGameScenarioDB> playerGameScenarios = await playerService.GetAllPlayerGameScenarios(gameScenarioId);

            List<Player> players = new List<Player>();

            foreach (PlayerDB player in await playerService.GetAllGameScenarioPlayers(gameScenarioId))
            {
                PlayerGameScenarioDB playerGameScenarioDB = playerGameScenarios.FirstOrDefault(pgs => pgs.PlayerId == player.Id);

                LocationDB location = null;
                
                if ((playerGameScenarioDB != null) && (playerGameScenarioDB.LocationId.HasValue)) location = locationDBs.FirstOrDefault(l => l.Id == playerGameScenarioDB.LocationId.Value);

                if (location == null) location = locationDBs.FirstOrDefault(l => l.StartingLocation);

                players.Add(new Player()
                {
                    ID = player.ExternalId,
                    Name = player.Name,
                    LocationId = location?.ExternalId,
                    Strength = player.Strength,
                    Health = player.Health,
                    Sanity = player.Sanity,
                    Resources = playerGameScenarioDB.Resources,
                    ResourcesPerTurn = player.ResourcesPerTurn
                });
            }

            return players;
        }

        [HttpPut, Route("/Players/{gameScenarioId}/Ready")]
        public async Task SetPlayerReady(string gameScenarioId)
        {
            PlayerService playerService = new PlayerService(_sytemLoggin, _dbConnectionString);

            bool startGame = await playerService.SetPlayerReady(gameScenarioId, playerId);

            Console.WriteLine("startGame: " + startGame);

            if (startGame)
            {
                GameScenarioService gameScenarioService = new GameScenarioService(_sytemLoggin, _dbConnectionString);

                GameScenarioDB gameScenario = await gameScenarioService.GetByExternalId(gameScenarioId);

                if (!string.IsNullOrEmpty(gameScenario.CurrentActor))
                {
                    Console.WriteLine("All ready");

                    PlayerDB player = await playerService.GetPlayer(gameScenarioId, gameScenario.CurrentActor);

                    if (player != null) await _gameHub.StartGame(gameScenarioId, player.ExternalId);
                }
            }
        }
    }
}
