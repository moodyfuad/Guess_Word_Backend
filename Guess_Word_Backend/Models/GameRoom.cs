using Microsoft.OpenApi.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Guess_Word_Backend.Models
{
    public class GameRoom
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Key { get; set; }

        public int WordLength { get; set; } = 5;
        public int MaxAttempts { get; set; } = 8;

        public Word? creatorWord{ get; set; }
        public Word? JonerWord{ get; set; }
        public Guid CreatorId { get; set; }
        public Guid? JoinerId { get; set; }
        public string CreatorName { get; set; } = string.Empty;
        public string JoinerName { get; set; } = string.Empty ;
        public string State { get; set; } = GameRoomStates.WaitingForPlayers;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    }
}
