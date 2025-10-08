namespace WordleServer.Dtos
{
    public class SubmitGuessRequestDto
    {
        public string GameKey { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string Guess { get; set; } = string.Empty;
    }
}
