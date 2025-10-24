using Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class PlayerEntity : BaseEntity
    {
        [Required]
        [Key]
        public required string Id { get; set; }
        public string? PicturePath { get; set; } = null;
        [Required]
        [Length(2, 20)]
        public string Name { get; set; } = string.Empty;
        public string? ConnectionId { get; set; }
        public bool IsOnline => ConnectionId != null;
        public int PlayedCount { get; set; } = 0;
        public int WinCount { get; set; } = 0;
        public PlayerStates State { get; set; } = PlayerStates.Available;
        public ICollection<PlayerEntity> Friends { get; set; } = [];

        [ForeignKey(nameof(RoomEntity.Key))]
        public string? CreatedRoomId { get; set; }

        [ForeignKey(nameof(RoomEntity.Key))]
        public string? JoinedRoomId { get; set; }

        [InverseProperty(nameof(RoomEntity.Creator))]
        public RoomEntity? CreatedRoom { get; set; }

        [InverseProperty(nameof(RoomEntity.Joiner))]
        public RoomEntity? JoinedRoom { get; set; }
    }
}
