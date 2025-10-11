namespace Guess_Word_Backend.Dtos
{
    public class CreateGameResponseDto
    {
        public CreateGameResponseDto(string gameKey, string creatorId, int wordLength, int maxAttempts)
        {
            GameKey = gameKey;
            CreatorId = creatorId;
            WordLength = wordLength;
            MaxAttempts = maxAttempts;
        }

        public string GameKey { get; set; } = string.Empty;
        public string CreatorId { get; set; }
        public int WordLength { get; set; }
        public int MaxAttempts { get; set; }
    }
}
