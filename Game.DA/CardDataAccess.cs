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
    public class CardDataAccess
    {
        private string _databaseConnectionString;

        private SystemLogging _systemLogging;

        public CardDataAccess(SystemLogging systemLogging, string databaseConnectionString)
        {
            _systemLogging = systemLogging;

            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<IList<CardDB>> GetByPlayer(string playerId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    return (await connection.QueryAsync<CardDB>("SELECT c.* FROM Player AS p " +
                                                                    "INNER JOIN PlayerCard AS pc ON p.Id = pc.PlayerId " +
                                                                    "INNER JOIN Card AS c ON pc.CardId = c.Id " +
                                                                    "WHERE p.ExternalId = @PlayerId", new { PlayerId = playerId })).ToList();
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetByScenarioId(" + playerId + "): Unable to retrieve Cards", ex);

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
