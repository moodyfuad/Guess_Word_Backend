using System.ComponentModel.DataAnnotations;

namespace Guess_Word_Backend.Models
{
    public class Letter
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int Index { get; set; }
        public string State { get; set; } = LetterStates.None;

        public Word? Word { get; set; }
    }
}
