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
    public class PlayerGameScenarioCardDataAccess
    {
        private string _databaseConnectionString;

        private SystemLogging _systemLogging;

        public PlayerGameScenarioCardDataAccess(SystemLogging systemLogging, string databaseConnectionString)
        {
            _systemLogging = systemLogging;

            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<IList<PlayerGameScenarioCardDB>> GetByPlayerGameScenarioCards(string gameScenarioId, string playerId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    return (await connection.QueryAsync<PlayerGameScenarioCardDB>("SELECT pgsc.* FROM GameScenario AS gs " +
                                                                    "INNER JOIN PlayerGameScenario AS pgs ON gs.Id = pgs.GameScenarioId " +
                                                                    "INNER JOIN Player AS p ON pgs.PlayerId = p.Id " +
                                                                    "INNER JOIN PlayerGameScenarioCard AS pgsc ON pgs.Id = pgsc.PlayerGameScenarioId " +
                                                                    "WHERE gs.ExternalId = @GameScenarioId AND p.ExternalId = @PlayerId", new { GameScenarioId = gameScenarioId, PlayerId = playerId })).ToList();
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetByPlayerGameScenarioCard(" + gameScenarioId + "): Unable to retrieve PlayerGameScenarioCards", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task Insert(int playerGameScenarioId, int cardId, int sequenceNo)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    await connection.ExecuteAsync("INSERT INTO PlayerGameScenarioCard (PlayerGameScenarioId, CardId, SequenceNo) VALUES (@PlayerGameScenarioId, @CardId, @SequenceNo)", 
                                                    new { PlayerGameScenarioId = playerGameScenarioId, CardId = cardId, SequenceNo = sequenceNo });
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("Insert(" + playerGameScenarioId + ", " + cardId + "," + sequenceNo + "): Unable to insert into PlayerGameScenarioCard", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task Delete(int playerGameScenarioId)
        {

            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    await connection.ExecuteAsync("DELETE FROM PlayerGameScenarioCard WHERE PlayerGameScenarioId = @PlayerGameScenarioId", new { PlayerGameScenarioId = playerGameScenarioId });
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("Delete(" + playerGameScenarioId + "): Unable to delete from PlayerGameScenarioCard", ex);

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
