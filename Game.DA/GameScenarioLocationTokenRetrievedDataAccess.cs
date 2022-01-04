using Dapper;
using Game.DB;
using Game.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Game.DA
{
    public class GameScenarioLocationTokenRetrievedDataAccess
    {
        private string _databaseConnectionString;

        private SystemLogging _systemLogging;

        public GameScenarioLocationTokenRetrievedDataAccess(SystemLogging systemLogging, string databaseConnectionString)
        {
            _systemLogging = systemLogging;

            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<IList<GameScenarioLocationTokenRetrievedDB>> GetAllByGameScenarioId(string gameScenarioId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    return (await connection.QueryAsync<GameScenarioLocationTokenRetrievedDB>("SELECT ltr.* FROM GameScenario AS gs " +
                                                                                            "INNER JOIN GameScenarioLocationTokenRetrieved AS ltr ON gs.Id = ltr.GameScenarioId " +
                                                                                            "WHERE gs.ExternalId = @GameScenarioId", new { GameScenarioId = gameScenarioId })).ToList();
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetAll(): Unable to retrieve players", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task<IList<GameScenarioLocationTokenRetrievedDB>> GetAllByLocationId(int locationId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    return (await connection.QueryAsync<GameScenarioLocationTokenRetrievedDB>("SELECT * FROM GameScenarioLocationTokenRetrieved WHERE LocationId = @LocationId", new { LocationId = locationId })).ToList();
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetAllByLocationId(): Unable to retrieve tokens", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task Insert(int gameScenarioId, int playerGameScenarioId, int locationId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    await connection.ExecuteAsync("INSERT INTO GameScenarioLocationTokenRetrieved (GameScenarioId, PlayerGameScenarioId, LocationId) VALUES (@GameScenarioId, @PlayerGameScenarioId, @LocationId)",
                                                    new { GameScenarioId = gameScenarioId, PlayerGameScenarioId = playerGameScenarioId, LocationId = locationId });
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("Insert(" + gameScenarioId + ", " + playerGameScenarioId + "," + locationId + "): Unable to insert into GameScenarioLocationTokenRetrieved", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
