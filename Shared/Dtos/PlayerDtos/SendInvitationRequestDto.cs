namespace Shared.Dtos.PlayerDtos
{
    public class SendInvitationRequestDto
    {
        public SendInvitationRequestDto(string fromPlayerId, string toPlayerId, int wordLength, int maxAttempts = 20)
        {
            FromPlayerId = fromPlayerId;
            ToPlayerId = toPlayerId;
            WordLength = wordLength;
            MaxAttempts = maxAttempts;
        }

        public string FromPlayerId { get; set; }
        public string ToPlayerId { get; set; }
        public int WordLength { get; set; }
        public int MaxAttempts { get; set; }
    }
}
