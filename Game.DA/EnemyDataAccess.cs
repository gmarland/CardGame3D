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
    public class EnemyDataAccess
    {
        private string _databaseConnectionString;

        private SystemLogging _systemLogging;

        public EnemyDataAccess(SystemLogging systemLogging, string databaseConnectionString)
        {
            _systemLogging = systemLogging;

            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<EnemyDB> GetById(int enemyId)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    List<EnemyDB> enemies = (await connection.QueryAsync<EnemyDB>("SELECT * FROM Enemy WHERE Id = @EnemyId", new { EnemyId = enemyId })).ToList();

                    if (enemies.Count > 0) return enemies.First();
                    else return null;
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetById(" + enemyId + "): Unable to retrieve Enemys", ex);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task<IList<EnemyDB>> GetAllById(IList<int> enemyIds)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    return (await connection.QueryAsync<EnemyDB>("SELECT * FROM Enemy WHERE Id IN @EnemyIds", new { EnemyIds = enemyIds.ToArray() })).ToList();
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetById(" + string.Join(",", enemyIds) + "): Unable to retrieve Enemys", ex);

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
