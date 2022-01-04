using CardGame.Models;
using Game.DB;
using Game.Logging;
using Game.SL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Website.Server.Controllers
{
    [ApiController, Route("[controller]")]
    public class AgendasController : ControllerBase
    {
        private SystemLogging _sytemLogging;

        private readonly string _dbConnectionString;

        public AgendasController(IConfiguration configuration, ILogger<LocationController> logger)
        {
            _dbConnectionString = configuration.GetConnectionString("dbConnectionString");

            _sytemLogging = new SystemLogging(logger);
        }

        [HttpGet, Route("/Agendas/{gameScenarioId}")]
        public async Task<IList<Agenda>> GetAllAgendas(string gameScenarioId)
        {
            AgendaService agendaService = new AgendaService(_sytemLogging, _dbConnectionString);

            List<Agenda> agendas = new List<Agenda>();

            foreach (AgendaDB agenda in await agendaService.GetAllGameScenarioAgendas(gameScenarioId))
            {
                agendas.Add(new Agenda()
                {
                    ID = agenda.ExternalId,
                    Title = agenda.Title,
                    Description = agenda.Description,
                    Completion = agenda.Completion,
                    RequiredTokens = agenda.RequiredTokens,
                    SequenceNo = agenda.SequenceNo
                });
            }

            return agendas;
        }
    }
}
