namespace Guess_Word_Backend.Hubs.HubDtos
{
    public class SendInvitationRequestDto
    {
        public string FromPlayerId { get; set; }
        public string ToPlayerId { get; set; }
        public int WordLength { get; set; }
    }
}
