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
    public class PlayerDataAccess
    {
        private string _databaseConnectionString;

        private SystemLogging _systemLogging;

        public PlayerDataAccess(SystemLogging systemLogging, string databaseConnectionString)
        {
            _systemLogging = systemLogging;

            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<PlayerDB> GetById(int playerId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    List<PlayerDB> players = (await connection.QueryAsync<PlayerDB>("SELECT p.* FROM Player AS p WHERE p.Id = @PlayerId", new { PlayerId = playerId })).ToList();

                    if (players.Count > 0) return players.First();
                    else return null;
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetById(" + playerId + "): Unable to retrieve player", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task<PlayerDB> GetByExternalId(string playerId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    List<PlayerDB> players = (await connection.QueryAsync<PlayerDB>("SELECT p.* FROM Player AS p WHERE p.ExternalId = @PlayerId", new { PlayerId = playerId })).ToList();

                    if (players.Count > 0) return players.First();
                    else return null;
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetByExternalId(" + playerId + "): Unable to retrieve player", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task<PlayerDB> GetGameScenarioPlayer(string gameScenarioId, string playerId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    List<PlayerDB> players = (await connection.QueryAsync<PlayerDB>("SELECT p.* FROM GameScenario AS gs " +
                                                                    "INNER JOIN PlayerGameScenario AS pgs ON gs.Id = pgs.GameScenarioId " +
                                                                    "INNER JOIN Player AS p ON pgs.PlayerId = p.Id " +
                                                                    "WHERE gs.ExternalId = @GameScenarioId AND p.ExternalId = @PlayerId", 
                                                                    new { GameScenarioId = gameScenarioId, PlayerId = playerId })).ToList();

                    if (players.Count > 0) return players.First();
                    else return null;
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetAll(): Unable to retrieve players", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task<IList<PlayerDB>> GetAllGameScenarioPlayers(string gameScenarioId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    return (await connection.QueryAsync<PlayerDB>("SELECT p.* FROM GameScenario AS gs " +
                                                                    "INNER JOIN PlayerGameScenario AS pgs ON gs.Id = pgs.GameScenarioId " +
                                                                    "INNER JOIN Player AS p ON pgs.PlayerId = p.Id " +
                                                                    "WHERE gs.ExternalId = @GameScenarioId", new { GameScenarioId = gameScenarioId })).ToList();
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetAll(): Unable to retrieve players", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task ApplyDamage(int playerId, int strengthDamage, int sanityDamage)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    await connection.ExecuteAsync("UPDATE Player SET Health = (Health-@StrengthDamage), Sanity = (Sanity-#SanityDamage) WHERE Id = @PlayerId", new { StrengthDamage = strengthDamage, SanityDamage = sanityDamage, PlayerId = playerId });
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("ApplyDamage(): Unable to update player", ex);

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
