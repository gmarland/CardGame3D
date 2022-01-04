namespace Game.DB
{
    public class LocationConnectionDB
    {
        public int Id { get; set; }

        public int ScenarioId { get; set; }

        public int SourceLocationId { get; set; }

        public int TargetLocationId { get; set; }
    }
}
