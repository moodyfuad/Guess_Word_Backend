using WordleServer.Models;

namespace WordleServer.Dtos
{
    public class GuessResultDto
    {
        public string Guess { get; set; } = string.Empty;
        public List<LetterState> Feedback { get; set; } = new();
        public int PlayerIndex { get; set; }
        public bool IsWinningGuess { get; set; }
    }
}
