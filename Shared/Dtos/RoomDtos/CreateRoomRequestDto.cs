namespace Shared.Dtos.RoomDtos
{
    public class CreateRoomRequestDto
    {
        public int WordLength { get; set; }
        public int MaxAttempts { get; set; }
        public string CreatorId { get; set; } = string.Empty;
        public string? CreatorName { get; set; }
    }
}
