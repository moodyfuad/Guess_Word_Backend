namespace WordleServer.Dtos
{
    public class CreateGameResponseDto
    {
        public string GameKey { get; set; } = string.Empty;
        public int WordLength { get; set; }
        public int MaxAttempts { get; set; }
    }
}
