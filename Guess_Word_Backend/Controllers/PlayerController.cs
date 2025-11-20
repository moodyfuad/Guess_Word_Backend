using Core.Services.Abstraction;
using Guess_Word_Backend.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Shared.Dtos.PlayerDtos;
using Shared.Helpers;

namespace WordleServer.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class PlayerController : ControllerBase
    {
        private readonly IHubContext<GameHub> _hubContext;
       
        private readonly IServiceManager _serviceManager;
        private readonly ILogger<PlayerController> _logger;

        public PlayerController(
            IHubContext<GameHub> hubContext,
            ILogger<PlayerController> logger,
            IServiceManager serviceManager)
        {
            this._hubContext = hubContext;
            _logger = logger;
            this._serviceManager = serviceManager;
        }

        [HttpGet("/api/players")]
        public async Task<ApiResponse<GetOnlinePlayersResponseDto>> GetOnlinePlayers([FromQuery] GetOnlinePlayersRequestDto dto)
        {
            var parameters = new PagedListRequestParameters
            {
                PageNumber = dto.PageNumber,
                PageSize = dto.PageSize
            };
            var response = await _serviceManager.PlayerService.GetOnlinePlayers(parameters);
            return ApiResponse<GetOnlinePlayersResponseDto>.Ok(response);
        }
        [HttpPost("Invite")]
        public async Task<ApiResponse<string>> InvitePlayer(SendInvitationRequestDto dto)
        {
            
            InvitePlayerResultDto result = await _serviceManager.PlayerService.InvitePlayer(dto);
            if (result.Success)
            {
            await _hubContext.Clients.Client(result.Receiver!.ConnectionId).SendAsync("OnInvitationReceived", result.Creator, dto);
            return ApiResponse<string>.Ok("Invitation Sent");

            }
            return ApiResponse<string>.BadRequest(result.Message);
        }
        [HttpPost("invite/response")]
        public async Task<ApiResponse<string>> ResponseToInvitation(SendInvitationResponseDto dto)
        {
            ResponseToInvitationResultDto result = await _serviceManager.PlayerService.ResponseToInvitation(dto);
            if (!result.Success)
            {
                return ApiResponse<string>.BadRequest(message: result.Message);
            }
            if(result.Room == null)
            {
                dto.State = InvitationStates.Rejected;
                await _hubContext.Clients.Client(result.Creator!.ConnectionId)
                  .SendAsync("OnInvitationRejected", dto.State);
                return ApiResponse<string>.Ok(InvitationStates.Rejected);
            }
            await _hubContext.Clients.Client(result.Creator.ConnectionId)
            .SendAsync("OnInvitationRejected", dto.State);
            await _hubContext.Clients.Clients([result.Creator!.ConnectionId, result.Joiner!.ConnectionId])
                  .SendAsync("OnGetsInvitationResponse", result.Room, result.Creator, result.Joiner);
            return ApiResponse<string>.Ok(InvitationStates.Accepted);
        }



    }
}
