using Game.DA;
using Game.DB;
using Game.Logging;
using System.Threading.Tasks;

namespace Game.SL
{
    public class GameScenarioService
    {
        private string _databaseConnectionString;

        private SystemLogging _systemLogging;

        public GameScenarioService(SystemLogging systemLogging, string databaseConnectionString)
        {
            _systemLogging = systemLogging;

            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<GameScenarioDB> GetByExternalId(string externalId)
        {
            GameScenarioDataAccess gameScenarioDataAccess = new GameScenarioDataAccess(_systemLogging, _databaseConnectionString);

            return await gameScenarioDataAccess.GetByExternalId(externalId);
        }

        public async Task SetCurrentTurn(int gameScenarioId, string currentActorId, int turnNo)
        {
            GameScenarioDataAccess gameScenarioDataAccess = new GameScenarioDataAccess(_systemLogging, _databaseConnectionString);

            await gameScenarioDataAccess.SetCurrentTurn(gameScenarioId, turnNo, currentActorId);
        }

        public async Task UpdateCurrentActor(int gameScenarioId, string currentActorId, string currentActorType)
        {
            GameScenarioDataAccess gameScenarioDataAccess = new GameScenarioDataAccess(_systemLogging, _databaseConnectionString);

            await gameScenarioDataAccess.UpdateCurrentActor(gameScenarioId, currentActorId, currentActorType);
        }

        public async Task EnemyDamagesPlayer(string gameScenarioId, string enemyId, string playerId)
        {
            GameScenarioDataAccess gameScenarioDataAccess = new GameScenarioDataAccess(_systemLogging, _databaseConnectionString);

            GameScenarioDB gameScenarioDB = await gameScenarioDataAccess.GetByExternalId(gameScenarioId);

            if (gameScenarioDB != null)
            {
                PlayerDataAccess playerDataAccess = new PlayerDataAccess(_systemLogging, _databaseConnectionString);

                PlayerDB player = await playerDataAccess.GetGameScenarioPlayer(gameScenarioId, playerId);

                if (player != null)
                {
                    GameScenarioEnemyDataAccess gameScenarioEnemyDataAccess = new GameScenarioEnemyDataAccess(_systemLogging, _databaseConnectionString);

                    GameScenarioEnemyDB gameScenarioEnemyDB = await gameScenarioEnemyDataAccess.Get(gameScenarioId, enemyId);

                    if (gameScenarioEnemyDB != null)
                    {
                        EnemyDataAccess enemyDataAccess = new EnemyDataAccess(_systemLogging, _databaseConnectionString);

                        EnemyDB enemy = await enemyDataAccess.GetById(gameScenarioEnemyDB.EnemyId);

                        if (enemy != null)
                        {
                            await playerDataAccess.ApplyDamage(player.Id, enemy.StrengthDamage, enemy.SanityDamage);
                        }
                    }
                }
            }
        }

        public async Task PlayerDamagesEnemy(string gameScenarioId, string playerId, string enemyId)
        {
            GameScenarioDataAccess gameScenarioDataAccess = new GameScenarioDataAccess(_systemLogging, _databaseConnectionString);

            GameScenarioDB gameScenarioDB = await gameScenarioDataAccess.GetByExternalId(gameScenarioId);

            if (gameScenarioDB != null)
            {
                GameScenarioEnemyDataAccess gameScenarioEnemyDataAccess = new GameScenarioEnemyDataAccess(_systemLogging, _databaseConnectionString);

                GameScenarioEnemyDB gameScenarioEnemyDB = await gameScenarioEnemyDataAccess.Get(gameScenarioId, enemyId);

                if (gameScenarioEnemyDB != null)
                {
                    PlayerDataAccess playerDataAccess = new PlayerDataAccess(_systemLogging, _databaseConnectionString);

                    PlayerDB player = await playerDataAccess.GetGameScenarioPlayer(gameScenarioId, playerId);

                    if (player != null)
                    {
                        await gameScenarioEnemyDataAccess.ApplyDamage(gameScenarioEnemyDB.Id, player.Strength);
                    }
                }
            }
        }

        public async Task<bool> PlayerStillAlive(string gameScenarioId, string playerId)
        {
            GameScenarioDataAccess gameScenarioDataAccess = new GameScenarioDataAccess(_systemLogging, _databaseConnectionString);

            GameScenarioDB gameScenarioDB = await gameScenarioDataAccess.GetByExternalId(gameScenarioId);

            if (gameScenarioDB != null)
            {
                PlayerDataAccess playerDataAccess = new PlayerDataAccess(_systemLogging, _databaseConnectionString);

                PlayerDB player = await playerDataAccess.GetGameScenarioPlayer(gameScenarioId, playerId);

                if (player != null)
                {
                    if ((player.Health <= 0) || (player.Sanity <= 0))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> EnemyStillAlive(string gameScenarioId, string enemyId)
        {
            GameScenarioDataAccess gameScenarioDataAccess = new GameScenarioDataAccess(_systemLogging, _databaseConnectionString);

            GameScenarioDB gameScenarioDB = await gameScenarioDataAccess.GetByExternalId(gameScenarioId);

            if (gameScenarioDB != null)
            {
                GameScenarioEnemyDataAccess gameScenarioEnemyDataAccess = new GameScenarioEnemyDataAccess(_systemLogging, _databaseConnectionString);

                GameScenarioEnemyDB gameScenarioEnemyDB = await gameScenarioEnemyDataAccess.Get(gameScenarioId, enemyId);

                if (gameScenarioEnemyDB != null)
                {
                    if (gameScenarioEnemyDB.Health <= 0)
                    {
                        await gameScenarioEnemyDataAccess.Delete(gameScenarioEnemyDB.Id);

                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
