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
    public class PlayerGraveDataAccess
    {
        private string _databaseConnectionString;

        private SystemLogging _systemLogging;

        public PlayerGraveDataAccess(SystemLogging systemLogging, string databaseConnectionString)
        {
            _systemLogging = systemLogging;

            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<IList<PlayerGraveDB>> GetByPlayerGrave(string gameScenarioId, string playerId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    return (await connection.QueryAsync<PlayerGraveDB>("SELECT pg.* FROM GameScenario AS gs " +
                                                                    "INNER JOIN PlayerGameScenario AS pgs ON gs.Id = pgs.GameScenarioId " +
                                                                    "INNER JOIN Player AS p ON pgs.PlayerId = p.Id " +
                                                                    "INNER JOIN PlayerGrave AS pg ON pgs.Id = pg.PlayerGameScenarioId " +
                                                                    "WHERE gs.ExternalId = @GameScenarioId AND p.ExternalId = @PlayerId", new { GameScenarioId = gameScenarioId, PlayerId = playerId })).ToList();
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetByPlayerGrave(" + gameScenarioId + "): Unable to retrieve PlayerGrave", ex);

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
