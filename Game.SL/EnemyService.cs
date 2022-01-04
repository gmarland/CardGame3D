using Game.DA;
using Game.DB;
using Game.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.SL
{
    public class EnemyService
    {
        private string _databaseConnectionString;

        private SystemLogging _systemLogging;

        public EnemyService(SystemLogging systemLogging, string databaseConnectionString)
        {
            _systemLogging = systemLogging;

            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<IList<GameScenarioEnemyDB>> GetAllGameScenarioEnemies(string gameScenarioId)
        {
            GameScenarioEnemyDataAccess gameScenarioEnemyDataAccess = new GameScenarioEnemyDataAccess(_systemLogging, _databaseConnectionString);

            return await gameScenarioEnemyDataAccess.GetAllByGameScenario(gameScenarioId);
        }

        public async Task<IList<EnemyDB>> GetAllEnemies(IList<int> enemyIds)
        {
            EnemyDataAccess enemyDataAccess = new EnemyDataAccess(_systemLogging, _databaseConnectionString);

            return await enemyDataAccess.GetAllById(enemyIds);
        }

        public async Task<IList<EnemyAbilityDB>> GetAllEnemyAbilities(IList<int> enemyIds)
        {
            EnemyAbilityDataAccess EnemyAbilityDataAccese = new EnemyAbilityDataAccess(_systemLogging, _databaseConnectionString);

            return await EnemyAbilityDataAccese.GetAll(enemyIds);
        }

        public async Task<GameScenarioEnemyDB> GetGameScenarioEnemy(string gameScenarioId, string gameScenarioEnemyId)
        {
            GameScenarioEnemyDataAccess gameScenarioEnemyDataAccess = new GameScenarioEnemyDataAccess(_systemLogging, _databaseConnectionString);

            return await gameScenarioEnemyDataAccess.Get(gameScenarioId, gameScenarioEnemyId);
        }

        public async Task UpdateEnemyLocation(string gameScenarioId, int gameScenarioEnemyId, int locationId)
        {
            GameScenarioDataAccess gameScenarioDataAccess = new GameScenarioDataAccess(_systemLogging, _databaseConnectionString);

            GameScenarioDB gameScenarioDB = await gameScenarioDataAccess.GetByExternalId(gameScenarioId);

            if (gameScenarioDB != null) await UpdateEnemyLocation(gameScenarioDB.Id, gameScenarioEnemyId, locationId);
        }

        public async Task UpdateEnemyLocation(int gameScenarioId, int gameScenarioEnemyId, int locationId)
        {
            GameScenarioEnemyDataAccess gameScenarioEnemyDataAccess = new GameScenarioEnemyDataAccess(_systemLogging, _databaseConnectionString);

            await gameScenarioEnemyDataAccess.UpdateLocation(gameScenarioId, gameScenarioEnemyId, locationId);
        }
    }
}
