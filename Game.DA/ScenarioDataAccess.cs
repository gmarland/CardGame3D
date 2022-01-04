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
    public class ScenarioDataAccess
    {
        private string _databaseConnectionString;

        private SystemLogging _systemLogging;

        public ScenarioDataAccess(SystemLogging systemLogging, string databaseConnectionString)
        {
            _systemLogging = systemLogging;

            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<ScenarioDB> GetByExternalId(string externalId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    List<ScenarioDB> scenarios = (await connection.QueryAsync<ScenarioDB>("SELECT * FROM Scenario WHERE ExternalId = @ExternalId", new { ExternalId = externalId })).ToList();

                    if (scenarios.Count > 0) return scenarios.First();
                    else return null;
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetByExternalId(" + externalId + "): Unable to retrieve Scenario", ex);

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
