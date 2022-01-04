namespace Game.DB
{
    public class PlayerGameScenarioDB
    {
        public int Id { get; set; }

        public int PlayerId { get; set; }

        public int GameScenarioId { get; set; }

        public int? LocationId { get; set; }

        public int Resources { get; set; }

        public bool Ready { get; set; }
    }
}
