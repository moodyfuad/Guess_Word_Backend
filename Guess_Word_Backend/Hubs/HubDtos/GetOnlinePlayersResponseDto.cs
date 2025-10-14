using Guess_Word_Backend.Models;

namespace Guess_Word_Backend.Hubs.HubDtos
{
    public class GetOnlinePlayersResponseDto : PagedList<PlayerDto>
    {
        public List<PlayerDto> Players { get => base.Items; }
        public GetOnlinePlayersResponseDto(int pageNumber, int pageSize, int totalCount, List<PlayerDto> items) : base(pageNumber, pageSize, totalCount, items)
        {

        }
    }
}
