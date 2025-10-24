namespace Shared.Dtos.RoomDtos
{
    public class SubmitGuessRequestDto
    {
        public SubmitGuessRequestDto(string gameKey, string clientId, string guess)
        {
            GameKey = gameKey;
            ClientId = clientId;
            Guess = guess;
        }

        public string GameKey { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string Guess { get; set; } = string.Empty;
    }
}
