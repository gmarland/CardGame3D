using System.Collections.Generic;

namespace CardGame.Models
{
    public class Location
    {
        public string ID { get; set; }

        public string Title { get; set; }

        public string RevealedDescription { get; set; }

        public bool StartingLocation { get; set; }

        public int XPos { get; set; }

        public int YPos { get; set; }

        public List<LocationStatCheck> LocationStatChecks { get; set; } = new List<LocationStatCheck>();

        public int NoOfTokens { get; set; }

        public List<string> ConnectedTo { get; set; } = new List<string>();
    }
}
