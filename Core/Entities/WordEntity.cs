using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class WordEntity : BaseEntity
    {
       
        [Required]
        //[Key]
        //public required string AsString { get => base.Id; set => base.Id = value; }
        [Key]
        public required string AsString { get ; set; }
        public int Length => AsString.Length;
    }
}
