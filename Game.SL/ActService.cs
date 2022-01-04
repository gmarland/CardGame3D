using Game.DA;
using Game.DB;
using Game.Logging;
using Game.SL.StatusUpdateEnums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.SL
{
    public class ActService
    {
        private string _databaseConnectionString;

        private SystemLogging _systemLogging;

        public ActService(SystemLogging systemLogging, string databaseConnectionString)
        {
            _systemLogging = systemLogging;

            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<ActDB> GetGameScenarioAct(string gameScenarioId)
        {
            ActDataAccess actDataAccess = new ActDataAccess(_systemLogging, _databaseConnectionString);

            return await actDataAccess.GetCurrentGameScenarioId(gameScenarioId);
        }

        public async Task<IList<ActDB>> GetAllGameScenarioActs(string gameScenarioId)
        {
            ActDataAccess actDataAccess = new ActDataAccess(_systemLogging, _databaseConnectionString);

            return await actDataAccess.GetByGameScenarioId(gameScenarioId);
        }

        public async Task<ActIncrease?> IncreaseActNumber(string gameScenarioId, string playerId, string locationId, int currentActNumber)
        {
            GameScenarioDataAccess gameScenarioDataAccess = new GameScenarioDataAccess(_systemLogging, _databaseConnectionString);

            GameScenarioDB gameScenarioDB = await gameScenarioDataAccess.GetByExternalId(gameScenarioId);

            if (gameScenarioDB != null)
            {
                PlayerGameScenarioDataAccess  playerGameScenarioData = new PlayerGameScenarioDataAccess(_systemLogging, _databaseConnectionString);

                PlayerGameScenarioDB playerGameScenario = await playerGameScenarioData.Get(playerId, gameScenarioId);

                if (playerGameScenario != null)
                {
                    LocationDataAccess locationDataAccess = new LocationDataAccess(_systemLogging, _databaseConnectionString);

                    LocationDB locationDB = await locationDataAccess.GetByExternalId(locationId);

                    if (locationDB != null)
                    {
                        GameScenarioLocationTokenRetrievedDataAccess gameScenarioLocationTokenRetrievedDataAccess = new GameScenarioLocationTokenRetrievedDataAccess(_systemLogging, _databaseConnectionString);

                        IList<GameScenarioLocationTokenRetrievedDB> retrievedTokens = await gameScenarioLocationTokenRetrievedDataAccess.GetAllByLocationId(locationDB.Id);

                        int availableTokens = locationDB.NoOfTokens - retrievedTokens.Count;

                        if (availableTokens > 0)
                        {
                            ActDataAccess actDataAccess = new ActDataAccess(_systemLogging, _databaseConnectionString);

                            ActDB actDB = await actDataAccess.GetById(gameScenarioDB.ActId);

                            if (actDB != null)
                            {
                                await gameScenarioLocationTokenRetrievedDataAccess.Insert(gameScenarioDB.Id, playerGameScenario.Id, locationDB.Id);

                                int newActTokens = (currentActNumber + 1);

                                if (newActTokens >= actDB.RequiredTokens)
                                {
                                    ActDB nextActDB = await actDataAccess.GetBySequenceNo(gameScenarioDB.ScenarioId, (actDB.SequenceNo + 1));

                                    if (nextActDB != null)
                                    {
                                        await gameScenarioDataAccess.UpdateActId(gameScenarioDB.Id, nextActDB.Id, 0);

                                        await gameScenarioDataAccess.IncreaseActionsUsed(gameScenarioDB.Id);

                                        return ActIncrease.ActProgressed;
                                    }
                                    else
                                    {
                                        await gameScenarioDataAccess.IncreaseActionsUsed(gameScenarioDB.Id);

                                        return ActIncrease.ActsComplete;
                                    }
                                }
                                else
                                {
                                    await gameScenarioDataAccess.UpdateActTokens(gameScenarioDB.Id, newActTokens);

                                    await gameScenarioDataAccess.IncreaseActionsUsed(gameScenarioDB.Id);

                                    return ActIncrease.ActTokenIncrease;
                                }
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
