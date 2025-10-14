using Guess_Word_Backend.Hubs.HubDtos;
using Guess_Word_Backend.Hubs.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace Guess_Word_Backend.Hubs.HubServices
{
    public static class RoomsService
    {
        public static RoomDto? GetRoomKey(string Key)
        {
            return HubData.Rooms.Find(r=> r.Key.Equals(Key, StringComparison.OrdinalIgnoreCase));
        }
        public static RoomDto? GetCreatorRoom(string creatorId)
        {
            return HubData.Rooms.Find(r=> r.CreatorId.Equals(creatorId));
        }
        public static RoomDto Create(PlayerDto creator,int wordlength,int maxAtempts)
        {
            DeleteCreatorRoom(creator);
            RoomDto room = new(GenerateGameKey(4), wordlength, maxAtempts, creator.Id);
            HubData.Rooms.Add(room);
            return room;
        }
        public static void DeleteCreatorRoom(PlayerDto creator)
        {
            var room = GetCreatorRoom(creator.Id);
            if (room != null)
            {
            HubData.Rooms.Remove(room);
                
            }
        }

        private static string GenerateGameKey(int len)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var sb = new StringBuilder();
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[len];
            rng.GetBytes(bytes);
            for (int i = 0; i < len; i++)
            {
                sb.Append(chars[bytes[i] % chars.Length]);
            }
            return sb.ToString();
        }

        public static RoomDto? GetJoinerRoom(string id)
        {
            return HubData.Rooms.Find(r => r?.JoinerId?.Equals(id)??false);
        }
    }
}
