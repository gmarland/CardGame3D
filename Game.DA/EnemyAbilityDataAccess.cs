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
    public class EnemyAbilityDataAccess
    {
        private string _databaseConnectionString;

        private SystemLogging _systemLogging;

        public EnemyAbilityDataAccess(SystemLogging systemLogging, string databaseConnectionString)
        {
            _systemLogging = systemLogging;

            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<IList<EnemyAbilityDB>> GetAll(IList<int> enemyIds)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnectionString))
            {
                try
                {
                    connection.Open();

                    return (await connection.QueryAsync<EnemyAbilityDB>("SELECT * " +
                                                                        "FROM EnemyAbility " +
                                                                        "WHERE EnemyId IN @EnemyIds", new { EnemyIds = enemyIds.ToArray() })).ToList();
                }
                catch (Exception ex)
                {
                    _systemLogging.Fatal("GetAll(): Unable to retrieve EnemyAbilitys", ex);

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
