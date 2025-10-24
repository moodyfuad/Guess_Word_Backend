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
    public class RoomEntity : BaseEntity
    {
        //public string Key { get => base.Id; set => base.Id = value; }
        [Key]
        public string Key { get; set; }
        [Required]
        [Range(2, 15)]
        public int WordLength { get; set; } = 5;

        [Required]
        [Range(2, 50)]
        public int MaxAttempts { get; set; } = 8;

        // creator
        [Required]
        public required PlayerEntity Creator { get; set; }
        [ForeignKey(nameof(Creator.Id))]
        public required string CreatorId { get; set; }

        [Length(2,10)]
        public string? CreatorWord { get; set; }
        // joiner
        public PlayerEntity? Joiner { get; set; } = null;

        [ForeignKey(nameof(Joiner.Id))]
        public string? JoinerId { get; set; }
        [Length(2,10)]
        public string? JoinerWord { get; set; }
        //todo: add room state
        public RoomStates State { get; set; } = RoomStates.Created;
    }
}
