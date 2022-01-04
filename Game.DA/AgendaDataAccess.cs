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
    public class AgendaDataAccess
    {
        private string _databaseConnectionString;

        private SystemLogging _systemLogging;

        public AgendaDataAccess(SystemLogging systemLogging, string databaseConnectionString)
        {
            _systemLogging = systemLogging;

            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<AgendaDB> GetById(int id)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    List<AgendaDB> agendas = (await connection.QueryAsync<AgendaDB>("SELECT * FROM Agenda WHERE Id = @AgendaId", new { AgendaId = id })).ToList();

                    if (agendas.Count > 0) return agendas.First();
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

        public async Task<AgendaDB> GetCurrentGameScenarioId(string gameScenarioId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    List<AgendaDB> agendas = (await connection.QueryAsync<AgendaDB>("SELECT a.* FROM GameScenario AS gs " +
                                                                            "INNER JOIN Agenda AS a ON gs.AgendaId = a.Id " +
                                                                            "WHERE gs.ExternalId = @GameScenarioId", new { GameScenarioId = gameScenarioId })).ToList();

                    if (agendas.Count > 0) return agendas.First();
                    else return null;
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetCurrentGameScenarioId(" + gameScenarioId + "): Unable to retrieve Agendas", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task<IList<AgendaDB>> GetByGameScenarioId(string gameScenarioId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    return (await connection.QueryAsync<AgendaDB>("SELECT a.* FROM GameScenario AS gs " +
                                                                    "INNER JOIN Scenario AS s ON gs.ScenarioId = s.Id " +
                                                                    "INNER JOIN Agenda AS a ON s.Id = a.ScenarioId " +
                                                                    "WHERE gs.ExternalId = @GameScenarioId", new { GameScenarioId = gameScenarioId })).ToList();
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetByScenarioId(" + gameScenarioId + "): Unable to retrieve Agendas", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task<AgendaDB> GetByExternalId(string externalId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    List<AgendaDB> agendas = (await connection.QueryAsync<AgendaDB>("SELECT * FROM Agenda WHERE ExternalId = @ExternalId", new { ExternalId = externalId })).ToList();

                    if (agendas.Count > 0) return agendas.First();
                    else return null;
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetExternalId(" + externalId + "): Unable to retrieve Agendas", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task<AgendaDB> GetBySequenceNo(int scenarioId, int sequenceNo)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    List<AgendaDB> agendas = (await connection.QueryAsync<AgendaDB>("SELECT * FROM Agenda WHERE ScenarioId = @ScenarioId AND SequenceNo = @SequenceNo", new { ScenarioId = scenarioId, SequenceNo = sequenceNo })).ToList();

                    if (agendas.Count > 0) return agendas.First();
                    else return null;
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetBySequenceNo(" + scenarioId + ", " + sequenceNo + "): Unable to retrieve Agenda", ex);

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
