namespace Guess_Word_Backend.Hubs.HubDtos
{
    public class PlayerDto
    {
        public PlayerDto(string id, string connectionId, string name, int playCount, int wingCount)
        {
            Id = id;
            ConnectionId = connectionId;
            Name = name;
            PlayCount = playCount;
            WinCount = wingCount;
        }

        public string Id { get; set; }
        public string Invitable { get; set; }
        public string ConnectionId { get; set; }
        public string Name {get;set;}
        public int PlayCount {get;set;}
        public int WinCount {get;set;}
    }
}
