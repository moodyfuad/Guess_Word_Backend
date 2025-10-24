using Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.PlayerDtos
{
    public class GetOnlinePlayersResponseDto : PagedList<PlayerDto>
    {
        public List<PlayerDto> Players { get => Items; }
        public GetOnlinePlayersResponseDto(int pageNumber, int pageSize, int totalCount, List<PlayerDto> items) : base(pageNumber, pageSize, totalCount, items)
        {

        }
    }
}
