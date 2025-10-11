namespace Guess_Word_Backend.Models
{
    public class Letter
    {
        public string Name { get; set; } = "";
        public int Index { get; set; }
        public string State { get; set; } = LetterStates.None;
    }
}
