namespace CardGame.Models
{
    public class Act : Card
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public int SequenceNo { get; set; }

        public int ProgressTokens { get; set; }
    }
}
