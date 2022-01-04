namespace CardGame.Models.Game
{
    public class Agenda : Card
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string ConculsionSummary { get; set; }

        public string ConculsionText { get; set; }

        public int SequenceNo { get; set; }

        public int ProgressTokens { get; set; }
    }
}
