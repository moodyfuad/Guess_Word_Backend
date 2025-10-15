using Guess_Word_Backend.Dtos;
using Guess_Word_Backend.Hubs;
using Guess_Word_Backend.Hubs.HubDtos;
using Guess_Word_Backend.Hubs.HubServices;
using Guess_Word_Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WordleServer.Controllers;

namespace Guess_Word_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IHubContext<GameHub> _hubContext;
        private readonly OnlinePlayersService _onlinePlayersService;
        private readonly RoomsService _roomsService;
        private readonly ILogger<PlayerController> _logger;

        public RoomController(
            IHubContext<GameHub> hubContext,
            OnlinePlayersService onlinePlayersService,
            RoomsService roomsService,
            ILogger<PlayerController> logger)
        {
            this._hubContext = hubContext;
            this._onlinePlayersService = onlinePlayersService;
            this._roomsService = roomsService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ApiResponse<RoomDto>> CreateRoom(CreateGameRoomRequestDto dto)
        {
            var creator = _onlinePlayersService.GetPlayerId(dto.CreatorId);

            if (creator is not null)
            {
                creator.Name = dto.CreatorName;
                var room = _roomsService.Create(creator, dto.WordLength, dto.MaxAttempts);

                return ApiResponse<RoomDto>.Created(room);
            }
            return ApiResponse<RoomDto>.BadRequest();

        }
        [HttpPost("join")]
        public async Task<ApiResponse<string>> JoinRoom(JoinRoomRequestDto dto)
        {
            var joinerPlayer = _onlinePlayersService.GetPlayerId(dto.JoinerId);
            var room = _roomsService.GetRoomKey(dto.GameKey);
            joinerPlayer.Name = dto.JoinerName;
            var creatorPlayer = _onlinePlayersService.GetPlayerId(room.CreatorId);
            if (room is null)
            {
                return ApiResponse<string>.BadRequest();
            }

            _onlinePlayersService.UpdatePlayer(joinerPlayer);

            room.JoinerId = dto.JoinerId;
            room.State = GameRoomStates.WaitingForWord;
            ;

            await _hubContext.Clients.Clients([
                joinerPlayer.ConnectionId,
               creatorPlayer.ConnectionId])
                .SendAsync("ReceiveGameRoomJoined", room, creatorPlayer, joinerPlayer);
            return ApiResponse<string>.Ok();
        }
        [HttpPost("submitWord")]
        public async Task<ApiResponse<string>> SubmitWord(SelectWordRequestDto dto)
        {
            RoomDto? room = _roomsService.GetRoomKey(dto.Roomkey);
            if (room == null || room.JoinerId == null)
            {
                return ApiResponse<string>.BadRequest();
            }
            var creator = _onlinePlayersService.GetPlayerId(room.CreatorId);
            var joiner = _onlinePlayersService.GetPlayerId(room.JoinerId);

            if (room is not null && joiner is not null && creator is not null)
            {
                if (room.CreatorId == dto.Id)
                {
                    room.CreatorWord = dto.Word;
                    await _hubContext.Clients.Client(joiner.ConnectionId).SendAsync("ReceiveOpponentSelectedItsWord", room.CreatorId, room.CreatorWord);

                }
                else if (room.JoinerId == dto.Id)
                {
                    room.JoinerWord = dto.Word;
                    await _hubContext.Clients.Client(creator.ConnectionId).SendAsync("ReceiveOpponentSelectedItsWord", room.JoinerId, room.JoinerWord);
                }
                return ApiResponse<string>.Ok();
            }
            return ApiResponse<string>.BadRequest();
        }

        [HttpPost("sendMyGuess")]
            public async Task<ApiResponse<string>> SendMyGuess(SendGuessRequestDto dto)
        {
            var room = _roomsService.GetRoomKey(dto.RoomKey);
            if (room == null)
            {
                return ApiResponse<string>.BadRequest(message: $"Room Not Found with key {dto.RoomKey}");
            }
            var receiverId = room.JoinerId == dto.SenderId ? room.CreatorId : room.JoinerId;
            var receiver = _onlinePlayersService.GetPlayerId(receiverId);

            if (receiver is null) return ApiResponse<string>.Ok();

            await _hubContext.Clients.Client(receiver.ConnectionId).SendAsync("ReceiveOpponentGuess", dto.SenderId, dto.Word);
            return ApiResponse<string>.Ok();
        }


    }
}
