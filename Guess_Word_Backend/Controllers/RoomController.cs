using Core.Services.Abstraction;
using Guess_Word_Backend.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Shared.Dtos.PlayerDtos;
using Shared.Dtos.RoomDtos;
using Shared.Helpers;
using WordleServer.Controllers;

namespace Guess_Word_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        // todo : set the players words when they join the room
        // todo : notify the other player that the opponent sat its word
        // todo : notify the players when both sat its word and redirect them to the game
        // todo : check the winner at the server and update db
        // todo : notify the players when they used all their attempts 
        // todo : let player increase the attemts when it reach its limit 

        // todo : send the guessed word to the opponent
        // todo : notify the player when opponent disconnected
        // todo : receive win request when opponent disconnected for 20 sec
        // todo : save player (play count & win count)
        // todo : handel the player win state
        // todo : handel the player lose state

        private readonly IHubContext<GameHub> _hubContext;
        private readonly IServiceManager _serviceManager;
        private readonly ILogger<PlayerController> _logger;

        public RoomController(
            IHubContext<GameHub> hubContext,
            IServiceManager serviceManager,
            ILogger<PlayerController> logger)
        {
            this._hubContext = hubContext;
            this._serviceManager = serviceManager;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ApiResponse<RoomDto>> CreateRoom(CreateRoomRequestDto dto)
        {
            RoomDto room = await _serviceManager.RoomService.CreateRoom(dto);
            return ApiResponse<RoomDto>.Created(room);
        }
        [HttpPost("join")]
        public async Task<ApiResponse<string>> JoinRoom(JoinRoomRequestDto dto)
        {
            ResponseToInvitationResultDto result = await _serviceManager.RoomService.JoinRoom(dto);
            if (!result.Success)
            {
                return ApiResponse<string>.BadRequest(result.Message);
            }

            await _hubContext.Clients.Clients([
                result.Joiner!.ConnectionId,
               result.Creator!.ConnectionId])
                .SendAsync("ReceiveGameRoomJoined", result.Room, result.Creator, result.Joiner);
            return ApiResponse<string>.Ok();
        }
        [HttpPost("submitWord")]
        public async Task<ApiResponse<string>> SubmitWord(SelectWordRequestDto dto)
        {
            SubmitWordResultDto submitResult = await _serviceManager.RoomService.SubmitWord(dto);
            if (!submitResult.Success)
            {
                return ApiResponse<string>.BadRequest(submitResult.Message);
            }
            if (submitResult.Room.CreatorId == dto.Id)
            {
               
                await _hubContext.Clients.Client(submitResult.JoinerConnectionId).
                    SendAsync("ReceiveOpponentSelectedItsWord", submitResult.Room.CreatorId, submitResult.Room.CreatorWord);
            }
            else if (submitResult.Room.JoinerId == dto.Id)
            {
                await _hubContext.Clients.Client(submitResult.CreatorConnectionId).
                    SendAsync("ReceiveOpponentSelectedItsWord", submitResult.Room.JoinerId, submitResult.Room.JoinerWord);
            }
            return ApiResponse<string>.Ok();
            
           
        }

        [HttpPost("sendMyGuess")]
        public async Task<ApiResponse<string>> SendMyGuess(SendGuessRequestDto dto)
        {
            SubmitWordResultDto result = await _serviceManager.RoomService.SubmitWordGuess(dto);
            if (!result.Success)
            {
                return ApiResponse<string>.BadRequest(message:result.Message);
            }
            string receiverConnectionId= result.Room!.JoinerId == dto.SenderId ? result.CreatorConnectionId : result.JoinerConnectionId;
            

            await _hubContext.Clients.Client(receiverConnectionId).SendAsync("ReceiveOpponentGuess", dto.SenderId, dto.Word);
            return ApiResponse<string>.Ok();
        }

        [HttpPost("leaveGame")]
        public async Task<ApiResponse<string>> LeaveGame(LeaveRoomRequestDto dto)
        {
            
            var result = await _serviceManager.RoomService.LeaveGame(dto);
            if (!string.IsNullOrEmpty(result))
            {
                await _hubContext.Clients.Client(result)
                    .SendAsync("ReceiveOpponentLeftGame");
            }
            return ApiResponse<string>.Ok();
        }

    }
}
