namespace WordleServer.Dtos
{
    public class SubmitSecretRequestDto
    {
        public string GameKey { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty; // plaintext: server will hash
    }
}
