using Microsoft.AspNetCore.SignalR;
using WordleServer.Dtos;

namespace WordleServer.Hubs
{
    public interface IGameClient
    {
        Task ReceiveGameStateAsync(GameStateDto state);
        Task ReceiveGuessResultAsync(GuessResultDto result);
    }

    public class GameHub : Hub<IGameClient>
    {
        // When client connects it should call JoinGroup with gameKey and clientId
        public async Task JoinGroup(string gameKey, string clientId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameKey);

            // Optionally: associate connection id with player in memory by calling service/repo
            // e.g., _playerConnectionStore[clientId] = Context.ConnectionId
        }

        public async Task LeaveGroup(string gameKey, string clientId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameKey);
            // remove mapping if stored
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // cleanup if you tracked connection mapping
            await base.OnDisconnectedAsync(exception);
        }
    }
}
