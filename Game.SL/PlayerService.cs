using Game.DA;
using Game.DB;
using Game.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Game.SL
{
    public class PlayerService
    {
        private string _databaseConnectionString;

        private SystemLogging _systemLogging;

        public PlayerService(SystemLogging systemLogging, string databaseConnectionString)
        {
            _systemLogging = systemLogging;

            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<PlayerDB> GetPlayer(int playerId)
        {
            PlayerDataAccess playerDataAccess = new PlayerDataAccess(_systemLogging, _databaseConnectionString);

            return await playerDataAccess.GetById(playerId);
        }

        public async Task<PlayerDB> GetPlayer(string gameScenarioId, string playerId)
        {
            PlayerDataAccess playerDataAccess = new PlayerDataAccess(_systemLogging, _databaseConnectionString);

            return await playerDataAccess.GetGameScenarioPlayer(gameScenarioId, playerId);
        }

        public async Task<PlayerGameScenarioDB> GetGameScenarioPlayer(string gameScenarioId, string playerId)
        {
            PlayerGameScenarioDataAccess playerGameScenarioDataAccess = new PlayerGameScenarioDataAccess(_systemLogging, _databaseConnectionString);

            return await playerGameScenarioDataAccess.Get(playerId, gameScenarioId);
        }

        public async Task<IList<PlayerDB>> GetAllGameScenarioPlayers(string gameScenarioId)
        {
            PlayerDataAccess playerDataAccess = new PlayerDataAccess(_systemLogging, _databaseConnectionString);

            return await playerDataAccess.GetAllGameScenarioPlayers(gameScenarioId);
        }

        public async Task<IList<PlayerGameScenarioDB>> GetAllPlayerGameScenarios(string gameScenarioId)
        {
            PlayerGameScenarioDataAccess playerGameScenarioDataAccess = new PlayerGameScenarioDataAccess(_systemLogging, _databaseConnectionString);

            return await playerGameScenarioDataAccess.GetAll(gameScenarioId);
        }

        public async Task<bool> SetPlayerReady(string gameScenarioId, string playerId)
        {
            GameScenarioDataAccess gameScenarioDataAccess = new GameScenarioDataAccess(_systemLogging, _databaseConnectionString);
            PlayerDataAccess playerDataAccess = new PlayerDataAccess(_systemLogging, _databaseConnectionString);

            GameScenarioDB gameScenario = await gameScenarioDataAccess.GetByExternalId(gameScenarioId);

            if (gameScenario != null)
            {
                PlayerDB player = await playerDataAccess.GetByExternalId(playerId);

                if (player != null)
                {
                    PlayerGameScenarioDataAccess playerGameScenarioDataAccess = new PlayerGameScenarioDataAccess(_systemLogging, _databaseConnectionString);

                    await playerGameScenarioDataAccess.SetPlayerGameScenarioReady(player.Id, gameScenario.Id);

                    bool allReady = true;

                    IList<PlayerGameScenarioDB> playerGameScenarioDBs = await playerGameScenarioDataAccess.GetAll(gameScenario.Id);

                    foreach (PlayerGameScenarioDB playerGameScenario in playerGameScenarioDBs)
                    {
                        if (!playerGameScenario.Ready)
                        {
                            allReady = false;
                            break;
                        }
                    }

                    if (allReady)
                    {
                        IList<PlayerDB> playerDBs = await playerDataAccess.GetAllGameScenarioPlayers(gameScenario.ExternalId);

                        await gameScenarioDataAccess.SetCurrentTurn(gameScenario.Id, 1, playerDBs.First().ExternalId);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    throw new Exception("Cannot find player " + playerId);
                }
            }
            else
            {
                throw new Exception("Cannot find game scenario " + gameScenarioId);
            }
        }

        public async Task SetPlayerLocation(string gameScenarioId, string playerId, string locationId)
        {
            GameScenarioDataAccess gameScenarioDataAccess = new GameScenarioDataAccess(_systemLogging, _databaseConnectionString);
            PlayerDataAccess playerDataAccess = new PlayerDataAccess(_systemLogging, _databaseConnectionString);
            LocationDataAccess locationDataAccess = new LocationDataAccess(_systemLogging, _databaseConnectionString);
            PlayerGameScenarioDataAccess playerGameScenarioDataAccess = new PlayerGameScenarioDataAccess(_systemLogging, _databaseConnectionString);

            GameScenarioDB gameScenarioDB = await gameScenarioDataAccess.GetByExternalId(gameScenarioId);
            PlayerDB playerDB = await playerDataAccess.GetByExternalId(playerId);
            LocationDB locationDB = await locationDataAccess.GetByExternalId(locationId);

            if ((gameScenarioDB != null) && (playerDB != null) && (locationDB != null))
            {
                await playerGameScenarioDataAccess.SetPlayerGameScenarioLocation(gameScenarioDB.Id, playerDB.Id, locationDB.Id);

                await gameScenarioDataAccess.IncreaseActionsUsed(gameScenarioDB.Id);
            }
        }
    }
}
