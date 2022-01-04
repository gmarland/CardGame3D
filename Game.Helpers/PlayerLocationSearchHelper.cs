using Game.DB;
using System.Collections.Generic;
using System.Linq;

namespace Game.Helpers
{
    public class TrackedLocation
    {
        public int LocationId { get; set; }

        public int Depth { get; set; }

        public IList<TrackedLocation> ParentLocations { get; set; } = new List<TrackedLocation>();

        public IList<int> PlayerIds { get; set; } = new List<int>();
    }

    public class PlayerLocationSearchHelper
    {
        public static IList<TrackedLocation> ShortestPath(int locationId, int depth, IList<TrackedLocation> parentLocations, IList<LocationConnectionDB> locationConnectionDBs, IList<PlayerGameScenarioDB> playerGameScenarioDBs)
        {
            List<LocationConnectionDB> connectedLocationDBs = locationConnectionDBs.Where(lc => (lc.SourceLocationId == locationId) || (lc.TargetLocationId == locationId)).ToList();

            List<int> connectedLocations = new List<int>();

            foreach (LocationConnectionDB connectedLocation in connectedLocationDBs)
            {
                int connected;

                if (connectedLocation.SourceLocationId == locationId) connected = connectedLocation.TargetLocationId;
                else connected = connectedLocation.SourceLocationId;

                if (!parentLocations.Any(tl => tl.LocationId == connected)) connectedLocations.Add(connected);
            }

            List<TrackedLocation> theseLocations = new List<TrackedLocation>();

            foreach (int connectedLocation in connectedLocations)
            {
                TrackedLocation trackedLocation = new TrackedLocation()
                {
                    LocationId = connectedLocation,
                    Depth = depth,
                    PlayerIds = playerGameScenarioDBs.Where(pgs => pgs.LocationId == connectedLocation).Select(pg => pg.Id).ToList()
                };

                foreach (TrackedLocation parentLocation in parentLocations) trackedLocation.ParentLocations.Add(parentLocation);

                theseLocations.Add(trackedLocation);
            }

            if (theseLocations.Any(tl => tl.PlayerIds.Count > 0))
            {
                return theseLocations;
            }
            else
            {
                List<TrackedLocation> allMyChildren = new List<TrackedLocation>();

                if (theseLocations.Count > 0)
                {
                    foreach (TrackedLocation thisLocation in theseLocations)
                    {
                        List<TrackedLocation> ammendedPath = new List<TrackedLocation>();

                        foreach (TrackedLocation parentLocation in parentLocations) ammendedPath.Add(parentLocation);
                        ammendedPath.Add(thisLocation);

                        allMyChildren.AddRange(ShortestPath(thisLocation.LocationId, (depth + 1), ammendedPath, locationConnectionDBs, playerGameScenarioDBs));
                    }
                }

                return allMyChildren;
            }
        }
    }
}
