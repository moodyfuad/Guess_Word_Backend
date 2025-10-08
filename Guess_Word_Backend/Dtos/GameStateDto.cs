namespace WordleServer.Dtos
{
    public class GameStateDto
    {
        public string Phase { get; set; } = string.Empty;
        public int? CurrentTurn { get; set; }
        public int WordLength { get; set; }
        public int MaxAttempts { get; set; }
        public List<string>? Players { get; set; } // e.g., names or client ids
        public List<GuessResultDto>? Guesses { get; set; }
    }
}
