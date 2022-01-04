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
    public class LocationDataAccess
    {
        private string _databaseConnectionString;

        private SystemLogging _systemLogging;

        public LocationDataAccess(SystemLogging systemLogging, string databaseConnectionString)
        {
            _systemLogging = systemLogging;

            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<LocationDB> GetByExternalId(string externalId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    List<LocationDB> locations = (await connection.QueryAsync<LocationDB>("SELECT * FROM Location WHERE ExternalId = @ExternalId", new { ExternalId = externalId })).ToList();

                    if (locations.Count > 0) return locations.First();
                    else return null;
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetByExternalId(" + externalId + "): Unable to retrieve GameScenario", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task<IList<LocationDB>> GetByGameScenarioId(string gameScenarioId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    return (await connection.QueryAsync<LocationDB>("SELECT l.* FROM GameScenario AS gs " +
                                                                    "INNER JOIN Scenario AS s ON gs.ScenarioId = s.Id " +
                                                                    "INNER JOIN Location AS l ON s.Id = l.ScenarioId " +
                                                                    "WHERE gs.ExternalId = @GameScenarioId", new { GameScenarioId = gameScenarioId })).ToList();
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetByScenarioId(" + gameScenarioId + "): Unable to retrieve Locations", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task<IList<LocationDB>> GetAll(IList<int> locationIds)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    return (await connection.QueryAsync<LocationDB>("SELECT * FROM Location WHERE Id IN @LocationIds", new { LocationIds = locationIds.ToArray() })).ToList();
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetAll(" + string.Join(",", locationIds) + "): Unable to retrieve GameScenario", ex);

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
