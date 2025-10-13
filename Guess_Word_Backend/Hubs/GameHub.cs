using Guess_Word_Backend.Dtos;
using Guess_Word_Backend.Models;
using Guess_Word_Backend.Services;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace Guess_Word_Backend.Hubs
{
   

    public class GameHub : Hub
    {
        //public GameHub(IGameService gameService)
        public GameHub()
        {
            //this._gameService = gameService;
            //_connectedPlayers = [];
        }
        
        private static ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();
        //private readonly IGameService _gameService;
        private static List<Player> _connectedPlayers = [];


        
        public override async Task OnConnectedAsync()
        {
            string connectionId = this.Context.ConnectionId;

            var http = this.Context.GetHttpContext();
            string userId = http?.Request.Query["userId"]!;


            _connectedPlayers.Add(
                   new()
                   {
                       ConnectionId = connectionId
                       ,
                       ClientId = userId.Trim('"','/','\\')
                       ,
                       Invitable = true

                   });
            await Clients.AllExcept([connectionId]).SendAsync("ReceiveOnlineUser", connectionId);
             foreach (Player player in _connectedPlayers)
            {
                if (player.ConnectionId == connectionId)
                {
                    continue;
                }
                await Clients.Caller.SendAsync("ReceiveOnlineUser",player.ConnectionId);
                
            }
         
                
            System.Console.WriteLine($"User {userId} Connected");


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

                _connectedPlayers.RemoveAll(p => p.ClientId == userId || p.ConnectionId == Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRoom(JoinGameRequestDto dto)
        {
           
            Player? player = _connectedPlayers.Find(p => p.ClientId == dto.JoinerId);
            Player? gameCreator = _connectedPlayers.Find(p => dto.GameKey.Equals(p.Room?.Key, StringComparison.OrdinalIgnoreCase));
            if (player is not null && gameCreator is not null && gameCreator.Room is not null)
            {
                player.Name = dto.JoinerName;
                gameCreator.Room.Joiner = new(player);
                gameCreator.Room.State = GameRoomStates.WaitingForWord;
            await this.Clients.Clients([player.ConnectionId,gameCreator.ConnectionId]).SendAsync("ReceiveGameRoomJoined",gameCreator.Room);
            }
        }
        public async Task SelectWord(SelectWordRequestDto dto)
        {
            Room? room = _connectedPlayers.Find(p => p.Room != null &&
            p.Room.Key == dto.Roomkey)?.Room;
            if (room is not null && room.Joiner is not null && room.Creator is not null)
            {
                if (room.Creator.ClientId == dto.Id)
                {
                    room.creatorWord = dto.Word;
                    await Clients.Client(room.Joiner!.ConnectionId).SendAsync("ReceiveOpponentSelectedItsWord",room.Creator.ClientId,room.creatorWord);

                }
                else if (room.Joiner is not null && room.Joiner.ClientId == dto.Id)
                {
                    room.JonerWord = dto.Word;
                    await Clients.Client(room.Creator!.ConnectionId).SendAsync("ReceiveOpponentSelectedItsWord", room.Joiner.ClientId, room.JonerWord);
                }
            }

        }
        public async Task SendMyGuess(string id,string guess)
        {
            await Clients.Others.SendAsync("ReceiveOpponentGuess", id, guess);
        }


        public async Task CreateRoom(CreateGameRoomRequestDto dto)
        {
            
            Player? creator = _connectedPlayers.Find(p => p.ConnectionId == Context.ConnectionId);
            if (creator is not null)
            {
                creator.Name = dto.CreatorName;
                creator.Room = new Room()
                {
                    Creator = new(creator),
                    Key = GenerateGameKey(4),
                    State = GameRoomStates.WaitingForPlayers,
                    MaxAttempts = dto.MaxAttempts,
                    WordLength = dto.WordLength,
                };
            }

            await Clients.Caller.SendAsync("ReceiveGameRoomCreated", creator.Room);
        }
        

        public async Task LeaveGroup(string gameKey, string clientId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameKey);
            // remove mapping if stored (not implemented here)
        }

        private static string GenerateGameKey(int len)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var sb = new StringBuilder();
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[len];
            rng.GetBytes(bytes);
            for (int i = 0; i < len; i++)
            {
                sb.Append(chars[bytes[i] % chars.Length]);
            }
            return sb.ToString();
        }
    }
}
