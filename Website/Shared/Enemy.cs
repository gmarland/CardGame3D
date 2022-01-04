using System.Collections.Generic;

namespace CardGame.Models
{
    public class Enemy
    {
        public string ID { get; set; }

        public string LocationId { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Health { get; set; }

        public int StrengthDamage { get; set; }

        public int SanityDamage { get; set; }

        public List<string> Abilities { get; set; } = new List<string>();
    }
}
