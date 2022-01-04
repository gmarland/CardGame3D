namespace Game.DB
{
    public class GameScenarioDB
    {
        public int Id { get; set; }

        public string ExternalId { get; set; }

        public int ScenarioId { get; set; }

        public int ActId { get; set; }
    
        public int AgendaId { get; set; }

        public int TurnNo { get; set; }

        public string CurrentActor { get; set; }

        public string CurrentActorType { get; set; }

        public int ActionsUsed { get; set; }

        public int ActTokens { get; set; }

        public int AgendaTokens { get; set; }
    }
}
