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
    public class ActDataAccess
    {
        private string _databaseConnectionString;

        private SystemLogging _systemLogging;

        public ActDataAccess(SystemLogging systemLogging, string databaseConnectionString)
        {
            _systemLogging = systemLogging;

            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<ActDB> GetById(int id)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    List<ActDB> acts = (await connection.QueryAsync<ActDB>("SELECT * FROM Act WHERE Id = @ActId", new { ActId = id })).ToList();

                    if (acts.Count > 0) return acts.First();
                    else return null;
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetById(" + id + "): Unable to retrieve Act", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task<ActDB> GetCurrentGameScenarioId(string gameScenarioId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    List<ActDB> acts = (await connection.QueryAsync<ActDB>("SELECT a.* FROM GameScenario AS gs " +
                                                                            "INNER JOIN Act AS a ON gs.ActId = a.Id " +
                                                                            "WHERE gs.ExternalId = @GameScenarioId", new { GameScenarioId = gameScenarioId })).ToList();

                    if (acts.Count > 0) return acts.First();
                    else return null;
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetCurrentGameScenarioId(" + gameScenarioId + "): Unable to retrieve Acts", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task<IList<ActDB>> GetByGameScenarioId(string gameScenarioId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    return (await connection.QueryAsync<ActDB>("SELECT a.* FROM GameScenario AS gs " +
                                                                "INNER JOIN Scenario AS s ON gs.ScenarioId = s.Id " +
                                                                "INNER JOIN Act AS a ON s.Id = a.ScenarioId " +
                                                                "WHERE gs.ExternalId = @GameScenarioId", new { GameScenarioId = gameScenarioId })).ToList();
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetByGameScenarioId(" + gameScenarioId + "): Unable to retrieve Acts", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task<ActDB> GetByExternalId(string externalId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    List<ActDB> acts = (await connection.QueryAsync<ActDB>("SELECT * FROM Act WHERE ExternalId = @ExternalId", new { ExternalId = externalId })).ToList();

                    if (acts.Count > 0) return acts.First();
                    else return null;
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetExternalId(" + externalId + "): Unable to retrieve Acts", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task<ActDB> GetBySequenceNo(int scenarioId, int sequenceNo)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    List<ActDB> acts = (await connection.QueryAsync<ActDB>("SELECT * FROM Act WHERE ScenarioId = @ScenarioId AND SequenceNo = @SequenceNo", new { ScenarioId = scenarioId, SequenceNo = sequenceNo })).ToList();

                    if (acts.Count > 0) return acts.First();
                    else return null;
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetBySequenceNo(" + scenarioId + ", " + sequenceNo + "): Unable to retrieve Act", ex);

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
