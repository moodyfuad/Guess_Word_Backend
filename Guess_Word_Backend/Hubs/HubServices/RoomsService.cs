using Guess_Word_Backend.Hubs.HubDtos;
using Guess_Word_Backend.Hubs.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace Guess_Word_Backend.Hubs.HubServices
{
    public  class RoomsService
    {
        private readonly HubData _data;

        public RoomsService(HubData data)
        {
            _data = data;
        }

        public  RoomDto? GetRoomKey(string Key)
        {
            return _data.Rooms.Find(r=> r.Key.Equals(Key, StringComparison.OrdinalIgnoreCase));
        }
        public  RoomDto? GetCreatorRoom(string creatorId)
        {
            return _data.Rooms.Find(r=> r.CreatorId.Equals(creatorId));
        }
        public  RoomDto Create(PlayerDto creator,int wordlength,int maxAtempts)
        {
            DeleteCreatorRoom(creator);
            RoomDto room = new(GenerateGameKey(4), wordlength, maxAtempts, creator.Id);
            _data.Rooms.Add(room);
            return room;
        }
        public  RoomDto ConfigerCteatorRoomForJoiner(PlayerDto creator,PlayerDto joiner)
        {
            RoomDto? room = GetCreatorRoom(creator.Id);
            if(room == null)
            {
               room = Create(creator, 5, 20);
               _data.Rooms.Add(room);
            } 
            room.JoinerId = joiner.Id;
            return room;
        }
        public  void DeleteCreatorRoom(PlayerDto creator)
        {
            var room = GetCreatorRoom(creator.Id);
            if (room != null)
            {
            _data.Rooms.Remove(room);
                
            }
        }

        private  string GenerateGameKey(int len)
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

        public  RoomDto? GetJoinerRoom(string id)
        {
            return _data.Rooms.Find(r => r?.JoinerId?.Equals(id)??false);
        }
    }
}
