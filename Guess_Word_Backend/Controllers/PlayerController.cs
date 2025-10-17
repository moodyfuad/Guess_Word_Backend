using Guess_Word_Backend.Dtos;
using Guess_Word_Backend.Hubs;
using Guess_Word_Backend.Hubs.HubDtos;
using Guess_Word_Backend.Hubs.HubServices;
using Guess_Word_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WordleServer.Dtos;

namespace WordleServer.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class PlayerController : ControllerBase
    {
        private readonly IHubContext<GameHub> _hubContext;
        private readonly OnlinePlayersService _onlinePlayersService;
        private readonly RoomsService _roomsService;
        private readonly ILogger<PlayerController> _logger;

        public PlayerController(
            IHubContext<GameHub> hubContext,
            OnlinePlayersService onlinePlayersService,
            ILogger<PlayerController> logger,
            RoomsService roomsService)
        {
            this._hubContext = hubContext;
            this._onlinePlayersService = onlinePlayersService;
            _logger = logger;
            _roomsService = roomsService;
        }

        [HttpGet("/api/players")]
        public async Task<ApiResponse<GetOnlinePlayersResponseDto>> GetOnlinePlayers(GetOnlinePlayersRequestDto dto)
        {

            var response = _onlinePlayersService.GetOnlinePlayers(dto);
            return ApiResponse<GetOnlinePlayersResponseDto>.Ok(response);
        }
        [HttpPost("Invite")]
        public async Task<ApiResponse<string>> InvitePlayer(SendInvitationRequestDto dto)
        {
            PlayerDto receiver = _onlinePlayersService.GetPlayerById(dto.ToPlayerId);
            PlayerDto creator = _onlinePlayersService.GetPlayerById(dto.FromPlayerId);

            _roomsService.Create(creator, dto.WordLength, 20);
            if (receiver == null) return ApiResponse<string>.BadRequest("this player is offline");
            await _hubContext.Clients.Client(receiver?.ConnectionId ?? "").SendAsync("OnInvitationReceived", creator);
            return ApiResponse<string>.Ok();
            
        }
        [HttpPost("invite/response")]
        public async Task<ApiResponse<string>> ResponseToInvitation(SendInvitationResponseDto dto)
        {
            PlayerDto inviterPlayer = _onlinePlayersService.GetPlayerById(dto.ToPlayerId);
            PlayerDto joinerPlayer = _onlinePlayersService.GetPlayerById(dto.FromPlayerId);
            if ( inviterPlayer == null || joinerPlayer == null)
            {
                return ApiResponse<string>.BadRequest(message: "Inviter left the game");
            }
            if (dto.State == InvitationStates.Accepted)
            {
                RoomDto room = _roomsService.ConfigerCteatorRoomForJoiner(inviterPlayer, joinerPlayer);
                await _hubContext.Clients.Clients([inviterPlayer.ConnectionId,joinerPlayer.ConnectionId])
                    .SendAsync("OnGetsInvitationResponse", room, inviterPlayer, joinerPlayer);
                await _hubContext.Clients.Client(inviterPlayer.ConnectionId)
                .SendAsync("OnInvitationRejected", dto.State);
                return ApiResponse<string>.Ok(InvitationStates.Accepted);
            }
            else
            {
                await _hubContext.Clients.Client(inviterPlayer.ConnectionId)
                   .SendAsync("OnInvitationRejected",dto.State);
                return ApiResponse<string>.Ok(InvitationStates.Rejected);
            }           
        }



    }
}
