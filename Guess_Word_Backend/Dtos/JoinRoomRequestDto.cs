namespace Guess_Word_Backend.Dtos
{
    public class JoinRoomRequestDto
    {
        public string GameKey { get; set; } = string.Empty;
        public string JoinerId { get; set; } = string.Empty; 
        public string? JoinerName { get; set; }
    }
}
