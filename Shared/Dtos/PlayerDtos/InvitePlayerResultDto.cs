using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.PlayerDtos
{
    public class InvitePlayerResultDto
    {
        public InvitePlayerResultDto(bool success, string message, PlayerDto? creator, PlayerDto? receiver)
        {
            Success = success;
            Message = message;
            Creator = creator;
            Receiver = receiver;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
        public PlayerDto? Creator { get; set; }
        public PlayerDto? Receiver { get; set; }
    }
}
