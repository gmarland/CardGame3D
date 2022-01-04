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
    public class LocationConnectionDataAccess
    {
        private string _databaseConnectionString;

        private SystemLogging _systemLogging;

        public LocationConnectionDataAccess(SystemLogging systemLogging, string databaseConnectionString)
        {
            _systemLogging = systemLogging;

            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<IList<LocationConnectionDB>> GetByGameScenarioId(string gameScenarioId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    return (await connection.QueryAsync<LocationConnectionDB>("SELECT l.* FROM GameScenario AS gs " +
                                                                            "INNER JOIN Scenario AS s ON gs.ScenarioId = s.Id " +
                                                                            "INNER JOIN LocationConnection AS l ON s.Id = l.ScenarioId " +
                                                                            "WHERE gs.ExternalId = @GameScenarioId", new { GameScenarioId = gameScenarioId })).ToList();
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetByScenarioId(" + gameScenarioId + "): Unable to retrieve LocationConnection", ex);

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
