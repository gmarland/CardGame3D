using Game.DA;
using Game.DB;
using Game.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Game.SL
{
    public class DeckService
    {
        private string _databaseConnectionString;

        private SystemLogging _systemLogging;

        public DeckService(SystemLogging systemLogging, string databaseConnectionString)
        {
            _systemLogging = systemLogging;

            _databaseConnectionString = databaseConnectionString;
        }

        // Player deck methods

        public async Task<IList<string>> GetPlayerDeck(string playerId, string gameScenarioId)
        {
            CardDataAccess cardDataAccess = new CardDataAccess(_systemLogging, _databaseConnectionString);
            PlayerGameScenarioCardDataAccess playerGameScenarioCardDataAccess = new PlayerGameScenarioCardDataAccess(_systemLogging, _databaseConnectionString);

            IList<CardDB> allCards = await cardDataAccess.GetByPlayer(playerId);

            List<string> playerDeck = new List<string>();

            foreach (PlayerGameScenarioCardDB deckCard in (await playerGameScenarioCardDataAccess.GetByPlayerGameScenarioCards(gameScenarioId, playerId)).OrderBy(h => h.SequenceNo))
            {
                CardDB card = allCards.FirstOrDefault(c => c.Id == deckCard.CardId);

                if (card != null) playerDeck.Add(card.ExternalId);
            }

            return playerDeck;
        }

        public async Task UpdatePlayerDeck(string playerId, string gameScenarioId, IList<string> cardIds)
        {
            PlayerGameScenarioDataAccess playerGameScenarioDataAccess = new PlayerGameScenarioDataAccess(_systemLogging, _databaseConnectionString);
            CardDataAccess cardDataAccess = new CardDataAccess(_systemLogging, _databaseConnectionString);
            PlayerGameScenarioCardDataAccess playerGameScenarioCardDataAccess = new PlayerGameScenarioCardDataAccess(_systemLogging, _databaseConnectionString);

            PlayerGameScenarioDB playerGameScenario = await playerGameScenarioDataAccess.Get(playerId, gameScenarioId);

            if (playerGameScenario != null)
            {
                IList<CardDB> allCards = await cardDataAccess.GetByPlayer(playerId);

                await playerGameScenarioCardDataAccess.Delete(playerGameScenario.Id);

                for (int i = 0; i < cardIds.Count; i++)
                {
                    CardDB card = allCards.FirstOrDefault(c => c.ExternalId == cardIds[i]);

                    if (card != null) await playerGameScenarioCardDataAccess.Insert(playerGameScenario.Id, card.Id, (i+1));
                }
            }
            else
            {
                throw new Exception("Unable to find player");
            }
        }

        // Player hand methods

        public async Task<IList<string>> GetPlayerHand(string playerId, string gameScenarioId)
        {
            CardDataAccess cardDataAccess = new CardDataAccess(_systemLogging, _databaseConnectionString);
            PlayerHandDataAccess playerHandDataAccess = new PlayerHandDataAccess(_systemLogging, _databaseConnectionString);

            IList<CardDB> allCards = await cardDataAccess.GetByPlayer(playerId);

            List<string> playerHand = new List<string>();

            foreach (PlayerHandDB handCard in (await playerHandDataAccess.GetByPlayerHand(gameScenarioId, playerId)).OrderBy(h => h.SequenceNo))
            {
                CardDB card = allCards.FirstOrDefault(c => c.Id == handCard.CardId);

                if (card != null) playerHand.Add(card.ExternalId);
            }

            return playerHand;
        }

        public async Task UpdatePlayerHand(string playerId, string gameScenarioId, IList<string> cardIds)
        {
            PlayerGameScenarioDataAccess playerGameScenarioDataAccess = new PlayerGameScenarioDataAccess(_systemLogging, _databaseConnectionString);
            CardDataAccess cardDataAccess = new CardDataAccess(_systemLogging, _databaseConnectionString);
            PlayerHandDataAccess playerHandDataAccess = new PlayerHandDataAccess(_systemLogging, _databaseConnectionString);

            PlayerGameScenarioDB playerGameScenario = await playerGameScenarioDataAccess.Get(playerId, gameScenarioId);

            if (playerGameScenario != null)
            {
                IList<CardDB> allCards = await cardDataAccess.GetByPlayer(playerId);

                await playerHandDataAccess.Delete(playerGameScenario.Id);

                for (int i = 0; i < cardIds.Count; i++)
                {
                    CardDB card = allCards.FirstOrDefault(c => c.ExternalId == cardIds[i]);

                    if (card != null) await playerHandDataAccess.Insert(playerGameScenario.Id, card.Id, (i + 1));
                }
            }
            else
            {
                throw new Exception("Unable to find player");
            }
        }
    }
}
