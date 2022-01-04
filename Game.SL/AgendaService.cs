using Game.DA;
using Game.DB;
using Game.Logging;
using Game.SL.StatusUpdateEnums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.SL
{
    public class AgendaService
    {
        private string _databaseConnectionString;

        private SystemLogging _systemLogging;

        public AgendaService(SystemLogging systemLogging, string databaseConnectionString)
        {
            _systemLogging = systemLogging;

            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<AgendaDB> GetGameScenarioAgenda(string gameScenarioId)
        {
            AgendaDataAccess agendaDataAccess = new AgendaDataAccess(_systemLogging, _databaseConnectionString);

            return await agendaDataAccess.GetCurrentGameScenarioId(gameScenarioId);
        }

        public async Task<IList<AgendaDB>> GetAllGameScenarioAgendas(string gameScenarioId)
        {
            AgendaDataAccess agendaDataAccess = new AgendaDataAccess(_systemLogging, _databaseConnectionString);

            return await agendaDataAccess.GetByGameScenarioId(gameScenarioId);
        }

        public async Task<AgendaIncrease?> IncreaseAgendaNumber(string gameScenarioId, int currentAgendaNumber)
        {
            GameScenarioDataAccess gameScenarioDataAccess = new GameScenarioDataAccess(_systemLogging, _databaseConnectionString);

            GameScenarioDB gameScenarioDB = await gameScenarioDataAccess.GetByExternalId(gameScenarioId);

            if (gameScenarioDB != null)
            {
                AgendaDataAccess agendaDataAccess = new AgendaDataAccess(_systemLogging, _databaseConnectionString);

                AgendaDB agendaDB = await agendaDataAccess.GetById(gameScenarioDB.AgendaId);

                if (agendaDB != null)
                {
                    int newAgendaTokens = (currentAgendaNumber + 1);

                    if (newAgendaTokens >= agendaDB.RequiredTokens)
                    {
                        AgendaDB nextAgendaDB = await agendaDataAccess.GetBySequenceNo(gameScenarioDB.ScenarioId, (agendaDB.SequenceNo + 1));

                        if (nextAgendaDB != null)
                        {
                            await gameScenarioDataAccess.UpdateAgendaId(gameScenarioDB.Id, nextAgendaDB.Id, 0);

                            return AgendaIncrease.AgendaProgressed;
                        }
                        else
                        {
                            return AgendaIncrease.AgendasComplete;
                        }
                    }
                    else
                    {
                        await gameScenarioDataAccess.UpdateAgendaTokens(gameScenarioDB.Id, newAgendaTokens);

                        return AgendaIncrease.AgendaTokenIncrease;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
