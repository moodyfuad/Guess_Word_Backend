namespace Guess_Word_Backend.Hubs.HubDtos
{
    public class SendInvitationResponseDto
    {
        public string FromPlayerId { get; set; }
        public string ToPlayerId { get; set; }
        public string State { get; set; }

        
    }
}
