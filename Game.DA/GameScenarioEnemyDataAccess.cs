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
    public class GameScenarioEnemyDataAccess
    {
        private string _databaseConnectionString;

        private SystemLogging _systemLogging;

        public GameScenarioEnemyDataAccess(SystemLogging systemLogging, string databaseConnectionString)
        {
            _systemLogging = systemLogging;

            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<IList<GameScenarioEnemyDB>> GetAllByGameScenario(string gameScenarioId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    return (await connection.QueryAsync<GameScenarioEnemyDB>("SELECT gse.* " +
                                                                            "FROM GameScenario AS gs " +
                                                                            "INNER JOIN GameScenarioEnemy AS gse ON gs.Id = gse.GameScenarioId " +
                                                                            "WHERE gs.ExternalId = @GameScenarioId", new { GameScenarioId = gameScenarioId })).ToList();
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetAllByGameScenario(" + gameScenarioId + "): Unable to retrieve GameScenarioEnemys", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task<GameScenarioEnemyDB> GetAllByGameScenario(string gameScenarioId, int enemyId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    IList<GameScenarioEnemyDB> gameScenarioEnemyDBs = (await connection.QueryAsync<GameScenarioEnemyDB>("SELECT gse.* " +
                                                                            "FROM GameScenario AS gs " +
                                                                            "INNER JOIN GameScenarioEnemy AS gse ON gs.Id = gse.GameScenarioId " +
                                                                            "WHERE gs.ExternalId = @GameScenarioId AND gse.Id = @EnemyId",
                                                                            new { GameScenarioId = gameScenarioId, EnemyId = enemyId })).ToList();

                    if (gameScenarioEnemyDBs.Count > 0) return gameScenarioEnemyDBs.First();
                    else return null;
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetAllByGameScenario(" + gameScenarioId + ", " + enemyId + "): Unable to retrieve GameScenarioEnemy", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task<GameScenarioEnemyDB> Get(string gameScenarioId, string gameScenarioEnemyId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    IList<GameScenarioEnemyDB> gameScenarioEnemyDBs = (await connection.QueryAsync<GameScenarioEnemyDB>("SELECT gse.* " +
                                                                            "FROM GameScenario AS gs " +
                                                                            "INNER JOIN GameScenarioEnemy AS gse ON gs.Id = gse.GameScenarioId " +
                                                                            "WHERE gs.ExternalId = @GameScenarioId AND gse.ExternalId = @GameScenarioEnemyId",
                                                                            new { GameScenarioId = gameScenarioId, GameScenarioEnemyId = gameScenarioEnemyId })).ToList();

                    if (gameScenarioEnemyDBs.Count > 0) return gameScenarioEnemyDBs.First();
                    else return null;
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetAllByGameScenario(" + gameScenarioId + ", " + gameScenarioEnemyId + "): Unable to retrieve GameScenarioEnemy", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task UpdateLocation(int gameScenarioId, int gameScenarioEnemyId, int locationId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    await connection.ExecuteAsync("UPDATE GameScenarioEnemy SET LocationId = @LocationId WHERE Id = @EnemyId AND GameScenarioId = @GameScenarioId", new { LocationId = locationId, EnemyId = gameScenarioEnemyId, GameScenarioId = gameScenarioId });
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("UpdateLocation(): Unable to update enemy", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task ApplyDamage(int gameScenarioEnemyId, int strengthDamage)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    await connection.ExecuteAsync("UPDATE GameScenarioEnemy SET Health = (Health-@StrengthDamage) WHERE Id = @GameScenarioEnemyId", new { StrengthDamage = strengthDamage, GameScenarioEnemyId = gameScenarioEnemyId });
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("ApplyDamage(): Unable to update enemy", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task Delete(int gameScenarioEnemyId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    await connection.ExecuteAsync("DELETE FROM GameScenarioEnemy WHERE Id = @GameScenarioEnemyId", new { GameScenarioEnemyId = gameScenarioEnemyId });
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("Delete(): Unable to update enemy", ex);

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
