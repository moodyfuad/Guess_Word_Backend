using Guess_Word_Backend.Dtos;
using Guess_Word_Backend.Services;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Guess_Word_Backend.Hubs
{
    //public interface IGameClient
    //{
    //    Task ReceiveGameStateAsync(GameStateDto state);
    //    Task ReceiveOnlineUser(string conId);
    //    Task ReceiveGuessResultAsync(GuessResultDto result);
    //    //Task ReceiveConnectionIdAsync(string connectionId);
    //    Task ReceiveGameRoomCreated(CreateGameRoomRequestDto connectionId);
    //    Task ReceiveGameRoomJoined(JoinGameRequestDto joinRequest);
    //}

    public class GameHub : Hub
    {
        // When client connects it should call JoinGroup with gameKey and clientId
        public GameHub(IGameService gameService)
        {
            this._gameService = gameService;
        }
        
        private static ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();
        private readonly IGameService _gameService;


        
        public override async Task OnConnectedAsync()
        {
            string connectionId = this.Context.ConnectionId;

            var http = this.Context.GetHttpContext();
            string? userId = http?.Request.Query["userId"].ToString();

            if (!string.IsNullOrWhiteSpace(userId))
            {
                _connections.AddOrUpdate(userId, connectionId, (_, __) => connectionId);
            }
            //await Clients.Client(connectionId).ReceiveConnectionIdAsync(connectionId).ConfigureAwait(false);

            
            await Clients.All.SendAsync("ReceiveOnlineUser",connectionId);

            await base.OnConnectedAsync();

        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var http = Context.GetHttpContext();
            string? userId = http?.Request.Query["userId"].ToString();

            if (!string.IsNullOrEmpty(userId))
            {
                // userId is checked for null/empty; use null-forgiving to satisfy analyzer
                _connections.TryRemove(userId!, out _);
                System.Console.WriteLine($"User {userId} disconnected");
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRoom(JoinGameRequestDto dto)
        {
            var result = await _gameService.JoinGameAsync(dto);
            if (result.Success && _connections.TryGetValue(result.CreatorId, out string? connectionId))
            {
                //todo: send joiner details to the creator 
                await this.Clients.Client(connectionId).SendAsync("ReceiveGameRoomJoined",dto);
                //todo: send creator details to the joiner 
                await this.Clients.Caller.SendAsync("ReceiveGameRoomJoined",dto);
            }
        }

        public async Task CreateRoom(CreateGameRoomRequestDto dto)
        {
            var result = await _gameService.CreateGameRoomAsync(dto);

            _connections.TryGetValue(result.CreatorId, out string? connectionId);

            connectionId ??= Context.ConnectionId;
            // Ensure we store the determined connectionId (non-null)
            _connections[result.CreatorId] = connectionId;

            await Clients.Caller.SendAsync("ReceiveGameRoomCreated", result);
        }
        

        public async Task LeaveGroup(string gameKey, string clientId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameKey);
            // remove mapping if stored (not implemented here)
        }
    }
}
