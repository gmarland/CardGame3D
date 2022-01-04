namespace Game.DB
{
    public class GameScenarioEnemyDB
	{
		public int Id { get; set; }

		public string ExternalId { get; set; }

		public int GameScenarioId { get; set; }

		public int EnemyId { get; set; }

		public int LocationId { get; set; }

		public int Health { get; set; }
	}
}
