using Shared.Dtos.PlayerDtos;
using Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Abstraction
{
    public interface IPlayerServices
    {
        Task<PlayerDto?> OnPlayerConnected(string connectionId, string playerId, string playerName, CancellationToken ct = default);
        Task <PlayerDto> OnPlayerDisconnected(string connectionId, CancellationToken ct = default);
        Task<GetOnlinePlayersResponseDto> GetOnlinePlayers(PagedListRequestParameters parameters, CancellationToken ct = default);
        Task<InvitePlayerResultDto> InvitePlayer(SendInvitationRequestDto dto, CancellationToken ct = default);

        Task<ResponseToInvitationResultDto> ResponseToInvitation(SendInvitationResponseDto dto, CancellationToken ct = default);
    }
}
