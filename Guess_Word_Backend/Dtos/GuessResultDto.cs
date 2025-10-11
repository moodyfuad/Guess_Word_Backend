using Guess_Word_Backend.Models;

namespace Guess_Word_Backend.Dtos
{
    public class GuessResultDto
    {
        public string Guess { get; set; } = string.Empty;
        public List<Letter> Letters{ get; set; } = [];
        public int PlayerIndex { get; set; }
        public bool IsWinningGuess { get; set; }
    }
}
