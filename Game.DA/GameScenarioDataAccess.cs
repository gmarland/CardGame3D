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
    public class GameScenarioDataAccess
    {
        private string _databaseConnectionString;

        private SystemLogging _systemLogging;

        public GameScenarioDataAccess(SystemLogging systemLogging, string databaseConnectionString)
        {
            _systemLogging = systemLogging;

            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<GameScenarioDB> GetByExternalId(string externalId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    List<GameScenarioDB> gameScenarios = (await connection.QueryAsync<GameScenarioDB>("SELECT * FROM GameScenario WHERE ExternalId = @ExternalId", new { ExternalId = externalId })).ToList();

                    if (gameScenarios.Count > 0) return gameScenarios.First();
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

        public async Task SetCurrentTurn(int id, int turnNo, string playerId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    await connection.ExecuteAsync("UPDATE GameScenario SET TurnNo = @TurnNo, CurrentActor = @CurrentActor, CurrentActorType = 'player', CurrentActorAction = 0, ActionsUsed = 0 WHERE Id = @GameScenarioId", 
                                                new { TurnNo = turnNo, CurrentActor = playerId, GameScenarioId = id });
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("SetCurrentTurn(" + id + "," + turnNo + "): Unable to update GameScenario", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task IncreaseActionsUsed(int id)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    await connection.ExecuteAsync("UPDATE GameScenario SET ActionsUsed = (ActionsUsed + 1) WHERE Id = @GameScenarioId", new { GameScenarioId = id });
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("UpdateActTokens(" + id + "): Unable to update GameScenario", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task UpdateCurrentActor(int id, string currentActorId, string currentActorType)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    await connection.ExecuteAsync("UPDATE GameScenario SET CurrentActor = @CurrentActor, ActionsUsed = 0, CurrentActorType = @CurrentActorType WHERE Id = @GameScenarioId", new { CurrentActor = currentActorId, GameScenarioId = id, CurrentActorType = currentActorType });
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("UpdateCurrentActor(" + id + "," + currentActorId + "): Unable to update GameScenario", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task UpdateActTokens(int id, int newActTokens)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    await connection.ExecuteAsync("UPDATE GameScenario SET ActTokens = @ActTokens WHERE Id = @GameScenarioId", new { ActTokens = newActTokens, GameScenarioId = id });
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("UpdateActTokens(" + id + "," + newActTokens + "): Unable to update GameScenario", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task UpdateActId(int id, int actId, int tokenCount)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    await connection.ExecuteAsync("UPDATE GameScenario SET ActId = @ActId, ActTokens = @ActTokens WHERE Id = @GameScenarioId", new { ActId = actId, ActTokens = tokenCount, GameScenarioId = id });
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("UpdateActId(" + id + "," + actId + "," + tokenCount + "): Unable to update GameScenario", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task UpdateAgendaTokens(int id, int newAgendaTokens)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    await connection.ExecuteAsync("UPDATE GameScenario SET AgendaTokens = @AgendaTokens WHERE Id = @GameScenarioId", new { AgendaTokens = newAgendaTokens, GameScenarioId = id });
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("UpdateActTokens(" + id + "," + newAgendaTokens + "): Unable to update GameScenario", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task UpdateAgendaId(int id, int agendaId, int tokenCount)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    await connection.ExecuteAsync("UPDATE GameScenario SET AgendaId = @AgendaId, AgendaTokens = @ActTokens WHERE Id = @GameScenarioId", new { AgendaId = agendaId, ActTokens = tokenCount, GameScenarioId = id });
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("UpdateAgendaId(" + id + "," + agendaId + "," + tokenCount + "): Unable to update GameScenario", ex);

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
