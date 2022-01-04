using Game.DA;
using Game.DB;
using Game.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.SL
{
    public class LocationService
    {
        private string _databaseConnectionString;

        private SystemLogging _systemLogging;

        public LocationService(SystemLogging systemLogging, string databaseConnectionString)
        {
            _systemLogging = systemLogging;

            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<IList<LocationDB>> GetAllGameScenarioLocations(string gameScenarioId)
        {
            LocationDataAccess locationDataAccess = new LocationDataAccess(_systemLogging, _databaseConnectionString);

            return await locationDataAccess.GetByGameScenarioId(gameScenarioId);
        }

        public async Task<IList<LocationDB>> GetAllLocations(IList<int> locationIds)
        {
            LocationDataAccess locationDataAccess = new LocationDataAccess(_systemLogging, _databaseConnectionString);

            return await locationDataAccess.GetAll(locationIds);
        }

        public async Task<IList<GameScenarioLocationTokenRetrievedDB>> GetAllGameScenarioLocationRetrievedTokens(string gameScenarioId)
        {
            GameScenarioLocationTokenRetrievedDataAccess gameScenarioLocationTokenRetrievedDataAccess = new GameScenarioLocationTokenRetrievedDataAccess(_systemLogging, _databaseConnectionString);

            return await gameScenarioLocationTokenRetrievedDataAccess.GetAllByGameScenarioId(gameScenarioId);
        }

        public async Task<IList<LocationConnectionDB>> GetAllGameScenarioLocationConnections(string gameScenarioId)
        {
            LocationConnectionDataAccess locationConnectionDataAccess = new LocationConnectionDataAccess(_systemLogging, _databaseConnectionString);

            return await locationConnectionDataAccess.GetByGameScenarioId(gameScenarioId);
        }
    }
}
