namespace Shared.Dtos.PlayerDtos
{
    public class SendInvitationResponseDto
    {
        public SendInvitationResponseDto(string fromPlayerId, string toPlayerId, string state)
        {
            FromPlayerId = fromPlayerId;
            ToPlayerId = toPlayerId;
            State = state;
        }

        public string FromPlayerId { get; set; }
        public string ToPlayerId { get; set; }
        public string State { get; set; }

        
    }
}
