namespace Game.DB
{
    public class EnemyDB
	{
		public int Id { get; set; }

        public string ExternalId { get; set; }

		public string Type { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public int Health { get; set; }

		public int StrengthDamage { get; set; }

		public int SanityDamage { get; set; }

	}
}
