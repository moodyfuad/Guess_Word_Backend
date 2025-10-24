namespace Shared.Dtos.RoomDtos
{
    public class SelectWordRequestDto
    {
        public SelectWordRequestDto(string roomkey, string id, string word)
        {
            Roomkey = roomkey;
            Id = id;
            Word = word;
        }

        public string Roomkey { get; set; }
        public string Id { get; set; }
        public string Word { get; set; }
    }
}
