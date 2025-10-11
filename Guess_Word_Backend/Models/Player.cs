using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Guess_Word_Backend.Models
{
    public class Player
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public bool Invitable { get; set; } = false;
        public string? Name { get; set; }

        [ForeignKey(nameof(GameRoom.Id))]
        public Guid? GameRoomId { get; set; }

        public GameRoom? GameRoom { get; set; }

        [Required]
        public string ClientId { get; set; } = string.Empty;

       
        [NotMapped]
        public string? ConnectionId { get; set; }
    }
}
