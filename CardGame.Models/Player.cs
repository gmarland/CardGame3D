using System.Collections.Generic;

namespace CardGame.Models
{
    public class Player
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public int ActionCount { get; set; }

        public List<Affinity> Affinities { get; set; } = new List<Affinity>();

        public List<PlayerHealthType> Health { get; set; } = new List<PlayerHealthType>();

        public List<PlayerStat> PlayerStats { get; set; } = new List<PlayerStat>();

        public List<Resource> Resources { get; set; } = new List<Resource>();
    }
}
