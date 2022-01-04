namespace Game.DB
{
    public class AgendaDB
    {
        public int Id { get; set; }

        public string ExternalId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Completion { get; set; }

        public int RequiredTokens { get; set; }

        public int SequenceNo { get; set; }
    }
}
