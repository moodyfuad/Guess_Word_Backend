namespace Shared.Dtos.RoomDtos
{
    public class CreateRoomResponseDto
    {
        public CreateRoomResponseDto(string gameKey, string creatorId, int wordLength, int maxAttempts, string creatorName)
        {
            RoomKey = gameKey;
            CreatorId = creatorId;
            WordLength = wordLength;
            MaxAttempts = maxAttempts;
            CreatorName = creatorName;
        }

        public string RoomKey { get; set; } = string.Empty;
        public string CreatorId { get; set; }
        public string CreatorName { get; set; }
        public int WordLength { get; set; }
        public int MaxAttempts { get; set; }
    }
}
