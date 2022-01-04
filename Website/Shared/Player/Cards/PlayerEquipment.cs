using System.Collections.Generic;

namespace CardGame.Models
{
    public class PlayerEquipment
    {
        public List<Affinity> Affinities { get; set; } = new List<Affinity>();

        public int Cost { get; set; }
    }
}
