namespace WordleServer.Dtos
{
    public class JoinGameResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int PlayerIndex { get; set; } // 0 or 1
    }
}
