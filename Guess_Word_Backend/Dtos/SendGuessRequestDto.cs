namespace Guess_Word_Backend.Dtos
{
    public class SendGuessRequestDto
    {
        public SendGuessRequestDto(string roomKey, string senderId, string word)
        {
            RoomKey = roomKey;
            SenderId = senderId;
            Word = word;
        }

        public string RoomKey { get; set; }
        public string SenderId { get; set; }
        public string Word { get; set; }
    }
}
