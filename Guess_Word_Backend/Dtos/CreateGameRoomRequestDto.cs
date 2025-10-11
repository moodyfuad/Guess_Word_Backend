namespace Guess_Word_Backend.Dtos
{
    public class CreateGameRoomRequestDto
    {
        public int WordLength { get; set; }
        public int MaxAttempts { get; set; }
        public string CreatorId { get; set; } = string.Empty;
        public string? CreatorName { get; set; }
    }
}
