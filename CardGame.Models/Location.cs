using System.Collections.Generic;

namespace CardGame.Models
{
    public class Location
    {
        public string ID { get; set; }

        public string Title { get; set; }

        public string RevealedDescription { get; set; }

        public List<LocationStatCheck> LocationStatChecks { get; set; } = new List<LocationStatCheck>();

        public int AgendaProgressTokens { get; set; }

        public List<string> ConnectedTo { get; set; } = new List<string>();
    }
}
