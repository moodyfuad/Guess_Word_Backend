using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        public Room? Room { get; set; }

        [Required]
        public string ClientId { get; set; } = string.Empty;

       
        [NotMapped]
        public string? ConnectionId { get; set; }
    }
    public class Room
    {
        public string Key { get; set; }

        public int WordLength { get; set; } = 5;
        public int MaxAttempts { get; set; } = 8;
        public string? creatorWord { get; set; }
        public string? JonerWord { get; set; }
        public PlayerNoRoom Creator { get; set; }
        public PlayerNoRoom? Joiner{ get; set; }
        public string State { get; set; } = GameRoomStates.WaitingForPlayers;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    }
    public class PlayerNoRoom
    {
      public PlayerNoRoom(Player player)
        {
            this.ClientId = player.ClientId;
            this.ConnectionId = player.ConnectionId;
            this.Invitable = player.Invitable;
            this.Name = player.Name;
            this.Room = player.Room;
        }
        public bool Invitable { get; set; } = false;
        public string? Name { get; set; }

        [JsonIgnore]
        public Room? Room { get; set; }

        public string ClientId { get; set; } = string.Empty;

        public string? ConnectionId { get; set; }
    }
}
