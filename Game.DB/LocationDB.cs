namespace Game.DB
{
    public class LocationDB
    {
        public int Id { get; set; }

        public string ExternalId { get; set; }

        public string Title { get; set; }

        public string RevealedDescription { get; set; }

        public bool StartingLocation { get; set; }

        public int NoOfTokens { get; set; }

        public int XPos { get; set; }

        public int YPos { get; set; }

    }
}
