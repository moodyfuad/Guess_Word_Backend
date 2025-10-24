namespace Shared.Dtos.RoomDtos
{
    public class RoomDto
    {
        public RoomDto(string key, int wordLength, int maxAttempts, string creatorId)
        {
            Key = key;
            WordLength = wordLength;
            MaxAttempts = maxAttempts;
            CreatorId = creatorId;
        }
        public RoomDto(string key, int wordLength, int maxAttempts, string? creatorWord, string? joinerWord, string creatorId, string? joinerId, string state, DateTime createdAt) : this(key, wordLength, maxAttempts, creatorWord)
        {
            Key = key;

            JoinerWord = joinerWord;
            CreatorWord = creatorWord;
            
            CreatorId = creatorId;
            JoinerId = joinerId;

            State = state;
            CreatedAt = createdAt;

            WordLength = WordLength;
            MaxAttempts = maxAttempts;

        }

        public string Key { get; set; }
        public int WordLength { get; set; } = 5;
        public int MaxAttempts { get; set; } = 8;
        public string? CreatorWord { get; set; }
        public string? JoinerWord { get; set; }
        public string CreatorId { get; set; }
        public string? JoinerId { get; set; }
        public string State { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
