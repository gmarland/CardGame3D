namespace Game.DB
{
    public class PlayerDB
    {
        public int Id { get; set; }

        public string ExternalId { get; set; }

        public string Name { get; set; }

        public int Strength { get; set; }

        public int Health { get; set; }

        public int Sanity { get; set; }

        public int ResourcesPerTurn { get; set; }
    }
}
