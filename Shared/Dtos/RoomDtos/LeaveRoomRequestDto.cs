using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.RoomDtos
{
    public class LeaveRoomRequestDto
    {
        public LeaveRoomRequestDto(string roomKey, string playerId)
        {
            RoomKey = roomKey;
            PlayerId = playerId;
        }

        public string RoomKey { get; set; }
        public string PlayerId { get; set; }
    }
}
