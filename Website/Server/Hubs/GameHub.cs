using Game.DB;
using Game.Logging;
using Game.SL;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Server.Hubs
{
    public class GameHub : Hub
    {
        public static string _databaseConnectionString;

        private static List<string> games = new List<string>();

        private SystemLogging _systemLogging;

        public GameHub(SystemLogging systemLogging, string databaseConnectionString)
        {
            _systemLogging = systemLogging;
            _databaseConnectionString = databaseConnectionString;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();

            string gameId = httpContext.Request.Query["gameId"];

            Console.WriteLine("gameId: " + gameId);

            GameScenarioService gameScenarioService = new GameScenarioService(_systemLogging, _databaseConnectionString);

            GameScenarioDB gameScenarioDB= await gameScenarioService.GetByExternalId(gameId);

            if (gameScenarioDB != null)
            {
                if (!games.Any(t => t.Equals(gameId))) games.Add(gameId);

                await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            foreach (string gameId in games) await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);

            await base.OnDisconnectedAsync(ex);
        }

        public async Task StartGame(string gameId, string playerId)
        {
            if (Clients != null) await Clients.Group(gameId).SendAsync("SetTurnNo", 1, playerId);
        }

        public async Task SetTurnNo(string gameId, int turnNo, string playerId)
        {
            if (Clients != null) await Clients.Group(gameId).SendAsync("SetTurnNo", turnNo, playerId);
        }

        public async Task ActTokenIncreased(string gameId, string locationId, string actId, int tokenNo)
        {
            if (Clients != null) await Clients.Group(gameId).SendAsync("ActTokenIncreased", locationId, actId, tokenNo);
        }

        public async Task ActProgressed(string gameId, string locationId, string newActId)
        {
            if (Clients != null) await Clients.Group(gameId).SendAsync("ActProgressed", locationId, newActId);
        }

        public async Task ActsComplete(string gameId)
        {
            if (Clients != null) await Clients.Group(gameId).SendAsync("AgendasComplete");
        }

        public async Task AgendaTokenIncreased(string gameId, string agendaId, int tokenNo)
        {
            if (Clients != null) await Clients.Group(gameId).SendAsync("AgendaTokenIncreased", agendaId, tokenNo);
        }

        public async Task AgendaProgressed(string gameId, string newAgendaId)
        {
            if (Clients != null) await Clients.Group(gameId).SendAsync("AgendaProgressed", newAgendaId);
        }

        public async Task AgendasComplete(string gameId)
        {
            if (Clients != null) await Clients.Group(gameId).SendAsync("AgendasComplete");
        }

        public async Task SetPlayerLocation(string gameId, string playerId, string locationId)
        {
            if (Clients != null) await Clients.Group(gameId).SendAsync("SetPlayerLocation", playerId, locationId);
        }

        public async Task SetEnemyLocation(string gameId, string enemyId, string locationId)
        {
            if (Clients != null) await Clients.Group(gameId).SendAsync("SetEnemyLocation", enemyId, locationId);
        }

        public async Task SetCurrentActor(string gameId, string actorId, string actorType)
        {
            if (Clients != null) await Clients.Group(gameId).SendAsync("SetCurrentActor", actorId, actorType);
        }

        public async Task PlayerDamagesEnemy(string gameId, string playerId, string enemyId, int newHealth)
        {
            if (Clients != null) await Clients.Group(gameId).SendAsync("PlayerDamagesEnemy", playerId, enemyId, newHealth);
        }

        public async Task EnemyDamagesPlayer(string gameId, string enemyId, string playerId, int newHealth, int newSanity)
        {
            if (Clients != null) await Clients.Group(gameId).SendAsync("EnemyDamagesPlayer", enemyId, playerId, newHealth, newSanity);
        }

        public async Task EnemyKillsPlayer(string gameId, string enemyId, string playerId)
        {
            if (Clients != null) await Clients.Group(gameId).SendAsync("EnemyKillsPlayer", enemyId, playerId);
        }

        public async Task DeleteEnemy(string gameId, string playerId, string enemyId)
        {
            if (Clients != null) await Clients.Group(gameId).SendAsync("DeleteEnemy", playerId, enemyId);
        }
    }
}
