namespace CardGame.Models
{
    public class Agenda
    {
        public string ID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Completion { get; set; }

        public int RequiredTokens { get; set; }

        public int SequenceNo { get; set; }
    }
}
