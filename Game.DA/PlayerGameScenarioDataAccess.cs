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
    public class PlayerGameScenarioDataAccess
    {
        private string _databaseConnectionString;

        private SystemLogging _systemLogging;

        public PlayerGameScenarioDataAccess(SystemLogging systemLogging, string databaseConnectionString)
        {
            _systemLogging = systemLogging;

            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<IList<PlayerGameScenarioDB>> GetAll(string gameScenarioId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    return (await connection.QueryAsync<PlayerGameScenarioDB>("SELECT pgs.* " + 
                                                                                "FROM PlayerGameScenario AS pgs INNER JOIN GameScenario AS gs ON pgs.GameScenarioId = gs.Id " + 
                                                                                "WHERE gs.ExternalId = @GameScenarioId", new { GameScenarioId = gameScenarioId })).ToList();
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetAll(): Unable to retrieve PlayerGameScenarios", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task<IList<PlayerGameScenarioDB>> GetAll(int gameScenarioId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    return (await connection.QueryAsync<PlayerGameScenarioDB>("SELECT * FROM PlayerGameScenario WHERE GameScenarioId = @GameScenarioId", new { GameScenarioId = gameScenarioId })).ToList();
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetAll(): Unable to retrieve PlayerGameScenarioss", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task<PlayerGameScenarioDB> Get(string playerId, string gameScenarioId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    List<PlayerGameScenarioDB> playerGameScenarioDB = (await connection.QueryAsync<PlayerGameScenarioDB>("SELECT pgs.* FROM PlayerGameScenario AS pgs " +
                                                                                                "INNER JOIN Player AS p ON pgs.PlayerId = p.Id " +
                                                                                                "INNER JOIN GameScenario AS gs ON pgs.GameScenarioId = gs.Id " +
                                                                                                "WHERE p.ExternalId = @PlayerId AND gs.ExternalId = @GameScenarioId",
                                                                                                new { PlayerId = playerId, GameScenarioId = gameScenarioId })).ToList();

                    if (playerGameScenarioDB.Count > 0) return playerGameScenarioDB.First();
                    else return null;
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetAll(): Unable to retrieve PlayerGameScenario", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task SetPlayerGameScenarioReady(int playerId, int gameScenarioId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    await connection.ExecuteAsync("UPDATE PlayerGameScenario SET Ready = 1 WHERE PlayerId = @PlayerId AND GameScenarioId = @GameScenarioId", new { PlayerId = playerId, GameScenarioId = gameScenarioId });
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("SetPlayerGameScenarioReady(): Unable to update player", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task SetPlayerGameScenarioLocation(int gameScenarioId, int playerId, int locationId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    await connection.ExecuteAsync("UPDATE PlayerGameScenario SET LocationId = @LocationId WHERE PlayerId = @PlayerId AND GameScenarioId = @GameScenarioId", new { LocationId = locationId, PlayerId = playerId, GameScenarioId = gameScenarioId });
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("SetPlayerGameScenarioLocation(): Unable to update player", ex);

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
