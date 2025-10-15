using Azure;
using Guess_Word_Backend.Dtos;
using Guess_Word_Backend.Hubs.HubDtos;
using Guess_Word_Backend.Hubs.HubServices;
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
        private static List<Player> _connectedPlayers = [];
        private readonly OnlinePlayersService _onlinePlayersService;
        private readonly RoomsService _roomsService;

        public GameHub(OnlinePlayersService onlinePlayersService, RoomsService roomsService)
        {
            _onlinePlayersService = onlinePlayersService;
            _roomsService = roomsService;
        }

        public override async Task OnConnectedAsync()
        {
            string connectionId = this.Context.ConnectionId;

            var http = this.Context.GetHttpContext();
            string userId = http?.Request.Query["userId"]!;
            string name = http?.Request.Query["name"] ?? "player";

            var newPlayer = _onlinePlayersService.AddPlayer(userId, connectionId, name);
           
            //await Clients.AllExcept([connectionId]).SendAsync("ReceiveOnlineUser", connectionId);
             foreach (PlayerDto player in _onlinePlayersService.GetPlayersRange(p=>p.ConnectionId != null))
            {
                if (player.ConnectionId == connectionId)
                {
                    continue;
                }
                await Clients.Caller.SendAsync("ReceiveOnlineUser",player.ConnectionId);
                
            }
            System.Console.WriteLine($"User {userId} Connected");
            await Clients.Others.SendAsync("OnNewPlayerConnected", newPlayer);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var http = Context.GetHttpContext();
            string? userId = http?.Request.Query["userId"].ToString();
            _handelOpponentDisconnected();
            PlayerDto player = _onlinePlayersService.GetPlayerConnectionId(Context.ConnectionId);
            await Clients.Others.SendAsync("OnPlayerDisconnected", player);

            _onlinePlayersService.RemoveBy(userId, Context.ConnectionId);
            System.Console.WriteLine($"User {userId} disconnected");
            await base.OnDisconnectedAsync(exception);
        }

        // controller
        //public async Task GetOnlinePlayers(GetOnlinePlayersRequestDto dto)
        //{
        //    var response = _onlinePlayersService.GetOnlinePlayers(dto);
        //    await Clients.Caller.SendAsync("OnReciveOnlinePlayers", response);
        //}

        //public async Task JoinRoom(JoinRoomRequestDto dto)
        //{
           
        //    var joinerPlayer = _onlinePlayersService.GetPlayerId(dto.JoinerId);
        //    var room = _roomsService.GetRoomKey(dto.GameKey);
        //    joinerPlayer.Name = dto.JoinerName;
        //    var creatorPlayer = _onlinePlayersService.GetPlayerId(room.CreatorId);

        //    _onlinePlayersService.UpdatePlayer(joinerPlayer);

        //    room.JoinerId = dto.JoinerId;
        //    room.State = GameRoomStates.WaitingForWord;
        //    ;
            
            
        //    await this.Clients.Clients([
        //        joinerPlayer.ConnectionId,
        //       creatorPlayer.ConnectionId])
        //        .SendAsync("ReceiveGameRoomJoined",room, creatorPlayer, joinerPlayer);            
        //}
        //public async Task SelectWord(SelectWordRequestDto dto)
        //{
        //    RoomDto? room = _roomsService.GetRoomKey(dto.Roomkey);
        //    var joiner = _onlinePlayersService.GetPlayerId(room.JoinerId);
        //    var creator = _onlinePlayersService.GetPlayerId(room.CreatorId);
        //    if (room is not null && joiner is not null && creator is not null)
        //    {
        //        if (room.CreatorId == dto.Id)
        //        {
        //            room.CreatorWord = dto.Word;
        //            await Clients.Client(joiner.ConnectionId).SendAsync("ReceiveOpponentSelectedItsWord",room.CreatorId,room.CreatorWord);

        //        }
        //        else if (room.JoinerId == dto.Id)
        //        {
        //            room.JoinerWord = dto.Word;
        //            await Clients.Client(creator.ConnectionId).SendAsync("ReceiveOpponentSelectedItsWord", room.JoinerId, room.JoinerWord);
        //        }
        //    }

        //}
        //public async Task SendMyGuess(string id,string guess)
        //{
        //    await Clients.Others.SendAsync("ReceiveOpponentGuess", id, guess);
        //}


        //public async Task CreateRoom(CreateGameRoomRequestDto dto)
        //{
        //    var creator = _onlinePlayersService.GetPlayerConnectionId(Context.ConnectionId);

        //    if (creator is not null)
        //    {
        //        creator.Name = dto.CreatorName;
        //        var room = _roomsService.Create(creator,dto.WordLength,dto.MaxAttempts);

        //    await Clients.Caller.SendAsync("ReceiveGameRoomCreated", room);
        //    }
        //}
        public async Task LeaveGame()
        {
            var player = _onlinePlayersService.GetPlayerConnectionId(Context.ConnectionId);
            var room = _roomsService.GetCreatorRoom(player?.Id??"")??
            _roomsService.GetJoinerRoom(player?.Id??"");
            if (room is null)
            {
                return;
            }
            var target = room.CreatorId == player.Id ? _onlinePlayersService.GetPlayerId(room.JoinerId??"") : _onlinePlayersService.GetPlayerId(room.CreatorId??"");
            if (target != null)
            {
                await Clients.Clients(target.ConnectionId)
                    .SendAsync("OnOpponentLeaveGame");
            }
        }

        //public async Task InvitePlayer(SendInvitationRequestDto dto)
        //{
        //    PlayerDto player = _onlinePlayersService.GetPlayerId(dto.ToPlayerId);
        //    if (player is not null)
        //    {
        //    await Clients.Clients(player?.ConnectionId??"").SendAsync("OnInvitationReceived", player);
        //    }
        //}
        public async Task ResponseToInvitation(SendInvitationResponseDto dto)
        {
            PlayerDto player = _onlinePlayersService.GetPlayerId(dto.ToPlayerId);
            if (player is not null)
            {
            await Clients.Clients(player?.ConnectionId??"").SendAsync("OnGetsInvitationResponse", dto);
            }
        }

        private void _handelOpponentDisconnected()
        {
            Player? disconnctedPlayer = _connectedPlayers.Find(p => p.ConnectionId == Context.ConnectionId);
            if (disconnctedPlayer != null && disconnctedPlayer.Room!= null && disconnctedPlayer.Room.Joiner != null)
            {
                string toConnectionId = string.Empty;
                if (disconnctedPlayer.Room.Creator.ConnectionId == Context.ConnectionId)
                {

                    toConnectionId = disconnctedPlayer.Room.Joiner.ConnectionId;
                }
                else
                {
                    toConnectionId = disconnctedPlayer.Room.Creator.ConnectionId;
                }

            Clients.Client(toConnectionId!).SendAsync("OnOpponentDisconnected");
            }

        }


        
    }
}
