using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.RoomDtos
{
    public class SubmitWordResultDto
    {
        public SubmitWordResultDto(bool success, string message, string creatorConnectionId, string joinerConnectionId, RoomDto? room = null)
        {
            Room = room;
            Success = success;
            Message = message;
            JoinerConnectionId = joinerConnectionId;
            CreatorConnectionId = creatorConnectionId;
        }

        public RoomDto? Room { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string JoinerConnectionId { get; set; }
        public string CreatorConnectionId { get; set; }
    }
}
