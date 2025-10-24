using Shared.Dtos.RoomDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.PlayerDtos
{
    public class ResponseToInvitationResultDto
    {
        public ResponseToInvitationResultDto(bool success, string message, PlayerDto? creator = null, PlayerDto? joiner = null, RoomDto? room = null)
        {
            Success = success;
            Message = message;
            Creator = creator;
            Joiner = joiner;
            Room = room;
        }

        public bool Success { get; set; }

        public string Message{ get; set; }
        public PlayerDto? Creator { get; set; }
        public PlayerDto? Joiner { get; set; }
        public RoomDto? Room{ get; set; }
    }
}
