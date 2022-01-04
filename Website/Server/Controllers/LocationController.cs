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

namespace Website.Server.Controllers
{
    [ApiController, Route("[controller]")]
    public class LocationController : ControllerBase
    {
        private SystemLogging _sytemLoggin;

        private readonly string _dbConnectionString;

        public LocationController(IConfiguration configuration, ILogger<LocationController> logger)
        {
            _dbConnectionString = configuration.GetConnectionString("dbConnectionString");

            _sytemLoggin = new SystemLogging(logger);
        }

        [HttpGet, Route("/Locations/{gameScenarioId}")]
        public async Task<IList<Location>> GetAllLocations(string gameScenarioId)
        {
            LocationService locationService = new LocationService(_sytemLoggin, _dbConnectionString);

            List<Location> locations = new List<Location>();

            IList<LocationDB> locationDBs = await locationService.GetAllGameScenarioLocations(gameScenarioId);
            IList<GameScenarioLocationTokenRetrievedDB> gameScenarioLocationTokenRetrievedDBs = await locationService.GetAllGameScenarioLocationRetrievedTokens(gameScenarioId);
            IList<LocationConnectionDB> locationConnectionDBs = await locationService.GetAllGameScenarioLocationConnections(gameScenarioId);

            foreach (LocationDB location in locationDBs)
            {
                List<string> connectedTo = new List<string>();

                List<LocationConnectionDB> connections = locationConnectionDBs.Where(lc => (lc.SourceLocationId == location.Id) || (lc.TargetLocationId == location.Id)).ToList();

                foreach (LocationConnectionDB connection in connections)
                {
                    LocationDB connectedLocation = null;

                    if (connection.SourceLocationId != location.Id) connectedLocation = locationDBs.FirstOrDefault(l => l.Id == connection.SourceLocationId);
                    else if (connection.TargetLocationId != location.Id) connectedLocation = locationDBs.FirstOrDefault(l => l.Id == connection.TargetLocationId);

                    if (connectedLocation != null) connectedTo.Add(connectedLocation.ExternalId);
                }

                int availableTokenCount = location.NoOfTokens - gameScenarioLocationTokenRetrievedDBs.Count;
                if (availableTokenCount < 0) availableTokenCount = 0;

                locations.Add(new Location()
                {
                    ID = location.ExternalId,
                    Title = location.Title,
                    RevealedDescription = location.RevealedDescription,
                    NoOfTokens = availableTokenCount,
                    StartingLocation = location.StartingLocation,
                    XPos = location.XPos,
                    YPos = location.YPos,
                    ConnectedTo = connectedTo
                });
            }

            return locations;
        }
    }
}
