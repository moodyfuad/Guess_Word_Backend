using Shared.Dtos.PlayerDtos;
using Shared.Dtos.RoomDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Abstraction
{
    public interface IRoomServices
    {
        Task<RoomDto> CreateRoom(CreateRoomRequestDto dto, CancellationToken ct = default);
        Task<ResponseToInvitationResultDto> JoinRoom(JoinRoomRequestDto dto, CancellationToken ct = default);
        Task<SubmitWordResultDto> SubmitWord(SelectWordRequestDto dto, CancellationToken ct = default);
        Task<SubmitWordResultDto> SubmitWordGuess(SendGuessRequestDto dto, CancellationToken ct = default);
        Task<string> LeaveGame(LeaveRoomRequestDto dto, CancellationToken ct = default);

    }
}
