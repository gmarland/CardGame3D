using CardGame.Models;
using Game.DB;
using Game.Helpers;
using Game.Logging;
using Game.SL;
using Game.SL.StatusUpdateEnums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Website.Server.Hubs;
using Website.Shared.Controllers;

namespace Website.Server.Controllers
{
    [ApiController, Route("[controller]")]
    public class GameScenarioController : ControllerBase
    {
        string playerId = "8934";

        private SystemLogging _sytemLogging;

        private readonly string _dbConnectionString;

        private GameHub _gameHub;

        public GameScenarioController(IConfiguration configuration, ILogger<LocationController> logger, GameHub gameHub)
        {
            _dbConnectionString = configuration.GetConnectionString("dbConnectionString");

            _sytemLogging = new SystemLogging(logger);

            _gameHub = gameHub;
        }

        [HttpGet, Route("/GameScenario/{gameScenarioId}")]
        public async Task<GameScenario> GetGameScenario(string gameScenarioId)
        {
            GameScenarioService gameScenarioService = new GameScenarioService(_sytemLogging, _dbConnectionString);
            PlayerService playerService = new PlayerService(_sytemLogging, _dbConnectionString);
            EnemyService enemyService = new EnemyService(_sytemLogging, _dbConnectionString);
            ActService actService = new ActService(_sytemLogging, _dbConnectionString);
            AgendaService agendaService = new AgendaService(_sytemLogging, _dbConnectionString);

            GameScenarioDB gameScenarioDB = await gameScenarioService.GetByExternalId(gameScenarioId);

            IList<ActDB> actDBs = await actService.GetAllGameScenarioActs(gameScenarioId);
            IList<AgendaDB> agendaDBs = await agendaService.GetAllGameScenarioAgendas(gameScenarioId);

            string currentActorType = gameScenarioDB.CurrentActorType;
            if (string.IsNullOrEmpty(currentActorType)) currentActorType = "player";

            if (string.IsNullOrEmpty(gameScenarioDB.CurrentActor))
            {
                if (currentActorType.Equals("player", StringComparison.OrdinalIgnoreCase))
                {
                    IList<PlayerDB> players = await playerService.GetAllGameScenarioPlayers(gameScenarioId);

                    gameScenarioDB.CurrentActor = players.First().ExternalId;
                    await gameScenarioService.UpdateCurrentActor(gameScenarioDB.Id, gameScenarioDB.CurrentActor, "player");
                }
                else
                {
                    IList<GameScenarioEnemyDB> gameScenarioEnemies = await enemyService.GetAllGameScenarioEnemies(gameScenarioId);

                    gameScenarioDB.CurrentActor = gameScenarioEnemies.First().ExternalId;
                    await gameScenarioService.UpdateCurrentActor(gameScenarioDB.Id, gameScenarioDB.CurrentActor, "enemy");
                }
            }

            return new GameScenario()
            {
                ID = gameScenarioDB.ExternalId,
                ActId = actDBs.FirstOrDefault(a => a.Id == gameScenarioDB.ActId).ExternalId,
                AgendaId = agendaDBs.FirstOrDefault(a => a.Id == gameScenarioDB.AgendaId).ExternalId,
                CurrentActor = gameScenarioDB.CurrentActor,
                CurrentActorType = gameScenarioDB.CurrentActorType,
                ActionsUsed = gameScenarioDB.ActionsUsed,
                TurnNo = gameScenarioDB.TurnNo,
                ActTokens = gameScenarioDB.ActTokens,
                AgendaTokens = gameScenarioDB.AgendaTokens
            };
        }


        [HttpPut, Route("/GameScenario/{gameScenarioId}/Location/{locationId}")]
        public async Task SetPlayerLocation(string gameScenarioId, string locationId)
        {
            PlayerService playerService = new PlayerService(_sytemLogging, _dbConnectionString);

            await playerService.SetPlayerLocation(gameScenarioId, playerId, locationId);

            await _gameHub.SetPlayerLocation(gameScenarioId, playerId, locationId);

            await ActPerformed(gameScenarioId);
        }

        [HttpPut, Route("/GameScenario/{gameScenarioId}/ActToken/{locationId}")]
        public async Task AddActToken(string gameScenarioId, string locationId, UpdateValueRequest advanceGameModel)
        {
            GameScenarioService gameScenarioService = new GameScenarioService(_sytemLogging, _dbConnectionString);
            ActService actService = new ActService(_sytemLogging, _dbConnectionString);

            ActIncrease? actIncreaseResult = await actService.IncreaseActNumber(gameScenarioId, playerId, locationId, advanceGameModel.CurrentValue);

            if (actIncreaseResult.HasValue)
            {
                GameScenarioDB gameScenarioDB = await gameScenarioService.GetByExternalId(gameScenarioId);
                ActDB actDB = await actService.GetGameScenarioAct(gameScenarioId);

                switch (actIncreaseResult.Value)
                {
                    case ActIncrease.ActTokenIncrease:
                        await _gameHub.ActTokenIncreased(gameScenarioId, locationId, actDB.ExternalId, gameScenarioDB.ActTokens);
                        break;
                    case ActIncrease.ActProgressed:
                        await _gameHub.ActProgressed(gameScenarioId, locationId, actDB.ExternalId);
                        break;
                    case ActIncrease.ActsComplete:
                        await _gameHub.ActsComplete(gameScenarioId);
                        break;
                }

                if (actIncreaseResult.Value != ActIncrease.ActsComplete) await ActPerformed(gameScenarioId);
            }
        }

        [HttpPut, Route("/GameScenario/{gameScenarioId}/AgendaToken")]
        public async Task AddAgendaToken(string gameScenarioId, UpdateValueRequest advanceGameModel)
        {
            AgendaService agendaService = new AgendaService(_sytemLogging, _dbConnectionString);

            AgendaIncrease? agendaIncreaseResult = await agendaService.IncreaseAgendaNumber(gameScenarioId, advanceGameModel.CurrentValue);

            if (agendaIncreaseResult.HasValue)
            {
                GameScenarioService gameScenarioService = new GameScenarioService(_sytemLogging, _dbConnectionString);

                GameScenarioDB gameScenarioDB = await gameScenarioService.GetByExternalId(gameScenarioId);
                AgendaDB agendaDB = await agendaService.GetGameScenarioAgenda(gameScenarioId);

                switch (agendaIncreaseResult.Value)
                {
                    case AgendaIncrease.AgendaTokenIncrease:
                        await _gameHub.AgendaTokenIncreased(gameScenarioId, agendaDB.ExternalId, gameScenarioDB.AgendaTokens);
                        break;
                    case AgendaIncrease.AgendaProgressed:
                        await _gameHub.AgendaProgressed(gameScenarioId, agendaDB.ExternalId);
                        break;
                    case AgendaIncrease.AgendasComplete:
                        await _gameHub.AgendasComplete(gameScenarioId);
                        break;
                }
            }
        }

        [HttpPut, Route("/GameScenario/{gameScenarioId}/ActPerformed")]
        public async Task ActPerformed(string gameScenarioId)
        {
            GameScenarioService gameScenarioService = new GameScenarioService(_sytemLogging, _dbConnectionString);
            PlayerService playerService = new PlayerService(_sytemLogging, _dbConnectionString);

            GameScenarioDB gameScenarioDB = await gameScenarioService.GetByExternalId(gameScenarioId);

            if (gameScenarioDB != null)
            {
                if (gameScenarioDB.ActionsUsed >= 3)
                {
                    new Thread(async () =>
                    {
                        IList<PlayerDB> players = await playerService.GetAllGameScenarioPlayers(gameScenarioId);

                        int? playerIndex = null;

                        for (int i = 0; i < players.Count; i++)
                        {
                            if (players[i].ExternalId == gameScenarioDB.CurrentActor)
                            {
                                playerIndex = i;
                                break;
                            }
                        }

                        if (!playerIndex.HasValue) playerIndex = 0;

                        if (playerIndex != (players.Count - 1))
                        {
                            await gameScenarioService.UpdateCurrentActor(gameScenarioDB.Id, players[(playerIndex.Value + 1)].ExternalId, "player");

                            await _gameHub.SetCurrentActor(gameScenarioDB.ExternalId, players[(playerIndex.Value + 1)].ExternalId, "player");
                        }
                        else
                        {
                            EnemyService enemyService = new EnemyService(_sytemLogging, _dbConnectionString);

                            IList<GameScenarioEnemyDB> gameScenarioEnemyDBs = await enemyService.GetAllGameScenarioEnemies(gameScenarioId);

                            foreach (GameScenarioEnemyDB gameScenarioEnemyDB in gameScenarioEnemyDBs)
                            {
                                await gameScenarioService.UpdateCurrentActor(gameScenarioDB.Id, gameScenarioEnemyDB.ExternalId, "enemy");

                                await _gameHub.SetCurrentActor(gameScenarioDB.ExternalId, gameScenarioEnemyDB.ExternalId, "enemy");

                                Thread.Sleep(new Random().Next(1,4)*500);

                                await MoveEnemyAction(gameScenarioId, gameScenarioEnemyDB.ExternalId);

                                Thread.Sleep(500);
                            }

                            await gameScenarioService.SetCurrentTurn(gameScenarioDB.Id, players[0].ExternalId, gameScenarioDB.TurnNo + 1);

                            await _gameHub.SetTurnNo(gameScenarioDB.ExternalId, (gameScenarioDB.TurnNo + 1), players[0].ExternalId);

                            await AddAgendaToken(gameScenarioId, new UpdateValueRequest()
                            {
                                CurrentValue = gameScenarioDB.AgendaTokens
                            });
                        }
                    }).Start();
                }
            }
        }

        [HttpPut, Route("/GameScenario/{gameScenarioId}/DamagePerformed")]
        public async Task DamagePerformed(string gameScenarioId, DamageRequest damageRequest)
        {
            GameScenarioService gameScenarioService = new GameScenarioService(_sytemLogging, _dbConnectionString);

            if (damageRequest.SourceType.Equals("Player", StringComparison.OrdinalIgnoreCase))
            {
                await gameScenarioService.PlayerDamagesEnemy(gameScenarioId, damageRequest.SourceId, damageRequest.TargetId);

                if (await gameScenarioService.EnemyStillAlive(gameScenarioId, damageRequest.TargetId))
                {
                    EnemyService enemyService = new EnemyService(_sytemLogging, _dbConnectionString);

                    GameScenarioEnemyDB gsEnemy = await enemyService.GetGameScenarioEnemy(gameScenarioId, damageRequest.TargetId);

                    if (gsEnemy != null) await _gameHub.PlayerDamagesEnemy(gameScenarioId, damageRequest.SourceId, damageRequest.TargetId, gsEnemy.Health);
                }
                else await _gameHub.DeleteEnemy(gameScenarioId, damageRequest.SourceId, damageRequest.TargetId);

                await ActPerformed(gameScenarioId);
            }
            else if (damageRequest.SourceType.Equals("Enemy", StringComparison.OrdinalIgnoreCase))
            {
                await gameScenarioService.EnemyDamagesPlayer(gameScenarioId, damageRequest.SourceId, damageRequest.TargetId);

                if (await gameScenarioService.PlayerStillAlive(gameScenarioId, damageRequest.TargetId))
                {
                    PlayerService playerService = new PlayerService(_sytemLogging, _dbConnectionString);

                    PlayerDB playerDB = await playerService.GetPlayer(gameScenarioId, damageRequest.TargetId);

                    if (playerDB != null) await _gameHub.EnemyDamagesPlayer(gameScenarioId, damageRequest.SourceId, damageRequest.TargetId, playerDB.Health, playerDB.Sanity);
                }
                else await _gameHub.EnemyKillsPlayer(gameScenarioId, damageRequest.SourceId, damageRequest.TargetId);
            }
        }

        public async Task MoveEnemyAction(string gameScenarioId, string enemyId)
        {
            EnemyService enemyService = new EnemyService(_sytemLogging, _dbConnectionString);
            PlayerService playerService = new PlayerService(_sytemLogging, _dbConnectionString);

            LocationService locationService = new LocationService(_sytemLogging, _dbConnectionString);

            IList<LocationDB> locationDBs = await locationService.GetAllGameScenarioLocations(gameScenarioId);
            IList<LocationConnectionDB> locationConnectionDBs = await locationService.GetAllGameScenarioLocationConnections(gameScenarioId);

            for (int i = 0; i < 2; i++)
            {
                GameScenarioEnemyDB gameScenarioEnemyDB = await enemyService.GetGameScenarioEnemy(gameScenarioId, enemyId);

                IList<PlayerGameScenarioDB> playerGameScenarioDBs = await playerService.GetAllPlayerGameScenarios(gameScenarioId);

                List<int> localPlayerIds = playerGameScenarioDBs.Where(pgs => pgs.LocationId == gameScenarioEnemyDB.LocationId).Select(pgs => pgs.Id).ToList();

                if (localPlayerIds.Count == 0)
                {
                    if (i == 0)
                    {
                        IList<TrackedLocation> trackedLocations = PlayerLocationSearchHelper.ShortestPath(gameScenarioEnemyDB.LocationId, 1, new List<TrackedLocation>()
                        {
                            new TrackedLocation()
                            {
                                LocationId = gameScenarioEnemyDB.LocationId,
                                Depth = 0
                            }
                        }, locationConnectionDBs, playerGameScenarioDBs);

                        trackedLocations = trackedLocations.Where(tl => tl.PlayerIds.Count > 0).ToList();

                        List<int> possibleLocations = new List<int>();

                        int minDepth = trackedLocations.Min(tl => tl.Depth);

                        if (minDepth > 1)
                        {
                            foreach (TrackedLocation playerLocation in trackedLocations.Where(tl => tl.Depth == minDepth))
                            {
                                if (playerLocation.ParentLocations.Count > 1) possibleLocations.Add(playerLocation.ParentLocations[1].LocationId);
                            }
                        }
                        else
                        {
                            foreach (TrackedLocation playerLocation in trackedLocations.Where(tl => tl.Depth == minDepth))
                            {
                                possibleLocations.Add(playerLocation.LocationId);
                            }
                        }

                        int nextLocationId = possibleLocations[new Random().Next(0, possibleLocations.Count)];

                        await enemyService.UpdateEnemyLocation(gameScenarioId, gameScenarioEnemyDB.Id, nextLocationId);

                        LocationDB nextLocation = locationDBs.FirstOrDefault(l => l.Id == nextLocationId);

                        if (nextLocation != null) await _gameHub.SetEnemyLocation(gameScenarioId, gameScenarioEnemyDB.ExternalId, nextLocation.ExternalId);
                    }
                }
                else
                {
                    int targetPlayerId = localPlayerIds[new Random().Next(0, (localPlayerIds.Count - 1))];

                    PlayerDB playerDB = await playerService.GetPlayer(targetPlayerId);

                    await DamagePerformed(gameScenarioId, new DamageRequest()
                    {
                        SourceType = "enemy",
                        SourceId = enemyId,
                        TargetType = "player",
                        TargetId = playerDB.ExternalId
                    });

                    break;
                }
            }
        }
    }
}
