using Game.DA;
using Game.DB;
using Game.Logging;
using System.Threading.Tasks;

namespace Game.SL
{
    public class ScenarioService
    {
        private string _databaseConnectionString;

        private SystemLogging _systemLogging;

        public ScenarioService(SystemLogging systemLogging, string databaseConnectionString)
        {
            _systemLogging = systemLogging;

            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<ScenarioDB> GetAllByExternalId(string externalId)
        {
            ScenarioDataAccess scenarioDataAccess = new ScenarioDataAccess(_systemLogging, _databaseConnectionString);

            return await scenarioDataAccess.GetByExternalId(externalId);
        }
    }
}
