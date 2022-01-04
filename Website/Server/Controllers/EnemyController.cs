using CardGame.Models;
using Game.DB;
using Game.Logging;
using Game.SL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Server.Hubs;

namespace Website.Server.Controllers
{
    [ApiController, Route("[controller]")]
    public class EnemyController : ControllerBase
    {
        private SystemLogging _sytemLogging;

        private readonly string _dbConnectionString;

        private GameHub _gameHub;

        public EnemyController(IConfiguration configuration, ILogger<EnemyController> logger, GameHub gameHub)
        {
            _dbConnectionString = configuration.GetConnectionString("dbConnectionString");

            _sytemLogging = new SystemLogging(logger);

            _gameHub = gameHub;
        }

        [HttpGet, Route("/Enemys/{gameScenarioId}")]
        public async Task<IList<Enemy>> GetEnemies(string gameScenarioId)
        {
            LocationService locationService = new LocationService(_sytemLogging, _dbConnectionString);
            EnemyService enemyService = new EnemyService(_sytemLogging, _dbConnectionString);

            IList<GameScenarioEnemyDB> gameScenarioEnemies = await enemyService.GetAllGameScenarioEnemies(gameScenarioId);

            List<int> locationIds = gameScenarioEnemies.Select(gs => gs.LocationId).Distinct().ToList();
            List<int> enemyIds = gameScenarioEnemies.Select(gs => gs.EnemyId).Distinct().ToList();

            IList<LocationDB> locationDBs = await locationService.GetAllLocations(locationIds);
            IList<EnemyDB> emenyDBs = await enemyService.GetAllEnemies(enemyIds);
            IList<EnemyAbilityDB> enemyAbilityDBs = await enemyService.GetAllEnemyAbilities(enemyIds);

            List<Enemy> enemies = new List<Enemy>();

            foreach (GameScenarioEnemyDB gameScenarioEnemy in gameScenarioEnemies)
            {
                LocationDB location = locationDBs.FirstOrDefault(l => l.Id == gameScenarioEnemy.LocationId);
                EnemyDB enemy = emenyDBs.FirstOrDefault(e => e.Id == gameScenarioEnemy.EnemyId);
                IList<EnemyAbilityDB> enemyAbilities = enemyAbilityDBs.Where(ea => ea.EnemyId == gameScenarioEnemy.EnemyId).ToList();

                if ((location != null) && (enemy != null))
                {
                    enemies.Add(new Enemy()
                    {
                        ID = gameScenarioEnemy.ExternalId,
                        LocationId = location.ExternalId,
                        Type = enemy.Type,
                        Name = enemy.Name,
                        Description = enemy.Description,
                        Health = gameScenarioEnemy.Health,
                        StrengthDamage = enemy.StrengthDamage,
                        SanityDamage = enemy.SanityDamage,
                        Abilities = enemyAbilities.Select(ea => ea.Name).Distinct().ToList()
                    });
                }
            }

            return enemies;
        }
    }
}
