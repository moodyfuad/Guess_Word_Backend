using Guess_Word_Backend.Dtos;
using Guess_Word_Backend.Services;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Guess_Word_Backend.Hubs
{
    public interface IGameClient
    {
        Task ReceiveGameStateAsync(GameStateDto state);
        Task ReceiveGuessResultAsync(GuessResultDto result);
        Task ReceiveConnectionIdAsync(string connectionId);
        Task ReceiveGameRoomCreated(CreateGameRoomRequestDto connectionId);
        Task ReceiveGameRoomJoined(JoinGameRequestDto joinRequest);
    }

    public class GameHub : Hub<IGameClient>
    {
        // When client connects it should call JoinGroup with gameKey and clientId
        public GameHub(IGameService gameService)
        {
            this._gameService = gameService;
        }

        private static readonly ConcurrentDictionary<string, string> _connections;
        private readonly IGameService _gameService;

        public override async Task OnConnectedAsync()
        {
            string connectionId = this.Context.ConnectionId;
            

            string? userId = this.Context.GetHttpContext()?.Request.Query["userId"] ?? "";

            if (!string.IsNullOrWhiteSpace(userId))
            {
                _connections[userId] = connectionId;
                await this.Clients.Caller.ReceiveConnectionIdAsync(connectionId);
            }

            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.GetHttpContext()?.Request.Query["userId"];
            if (!string.IsNullOrEmpty(userId))
            {
                _connections.TryRemove(userId, out _);
                System.Console.WriteLine($"User {userId} disconnected");
            }
            await base.OnDisconnectedAsync(exception);
        }
        public async Task JoinRoom(JoinGameRequestDto dto)
        {
            var reslut = await _gameService.JoinGameAsync(dto);
            if (reslut.Success && _connections.TryGetValue(reslut.CreatorId, out string? connectionId))
            {
                await this.Clients.Client(connectionId).ReceiveGameRoomJoined(dto);
            }

        }
        public async Task CreateRoom(CreateGameRoomRequestDto dto)
        {
            var reslut = await _gameService.CreateGameRoomAsync(dto);
            _connections.TryGetValue(reslut.CreatorId, out string? connectionId);
            
                connectionId ??= Context.ConnectionId;
                _connections[reslut.CreatorId] = connectionId;

            await Clients.All.ReceiveGameRoomCreated(dto);
        }
        

        public async Task LeaveGroup(string gameKey, string clientId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameKey);
            // remove mapping if stored
        }

        
    }
}
