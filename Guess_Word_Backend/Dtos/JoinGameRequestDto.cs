namespace WordleServer.Dtos
{
    public class JoinGameRequestDto
    {
        public string GameKey { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty; // unique per client
        public string? DisplayName { get; set; }
    }
}
