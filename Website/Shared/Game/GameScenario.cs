namespace CardGame.Models
{
    public class GameScenario
    {
        public string ID { get; set; }

        public string CurrentActor { get; set; }

        public string CurrentActorType { get; set; }

        public int ActionsUsed { get; set; }

        public string ActId { get; set; }

        public string AgendaId { get; set; }

        public int TurnNo { get; set; }

        public int ActTokens { get; set; }

        public int AgendaTokens { get; set; }
    }
}
