using System.Collections.Generic;

namespace CardGame.Models
{
    public class Player
    {
        public string ID { get; set; }

        public string LocationId { get; set; }

        public string Name { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public int ActionCount { get; set; }

        public int Strength { get; set; }

        public int Health { get; set; }

        public int Sanity { get; set; }

        public int Resources { get; set; }

        public int ResourcesPerTurn { get; set; }

        public List<Affinity> Affinities { get; set; } = new List<Affinity>();
    }
}
