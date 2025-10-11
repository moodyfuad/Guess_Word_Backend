namespace WordleServer.Dtos
{
    public class JoinGameResponseDto
    {
        public JoinGameResponseDto(bool success, string message, string creatorId, string creatorName)
        {
            Success = success;
            Message = message;
            CreatorId = creatorId;
            CreatorName = creatorName;
        }

        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string CreatorId { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
        //public int PlayerIndex { get; set; } // 0 or 1
    }
}
