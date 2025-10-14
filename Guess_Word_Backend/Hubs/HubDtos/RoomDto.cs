using Guess_Word_Backend.Models;

namespace Guess_Word_Backend.Hubs.HubDtos
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

        public string Key { get; set; }
        public int WordLength { get; set; } = 5;
        public int MaxAttempts { get; set; } = 8;
        public string? CreatorWord { get; set; }
        public string? JoinerWord { get; set; }
        public string CreatorId { get; set; }
        public string? JoinerId { get; set; }
        public string State { get; set; } = GameRoomStates.WaitingForPlayers;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
