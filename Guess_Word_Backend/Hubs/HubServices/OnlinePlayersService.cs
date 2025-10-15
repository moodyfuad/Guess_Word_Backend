using Guess_Word_Backend.Hubs.HubDtos;
using Guess_Word_Backend.Hubs.Repositories;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Linq.Expressions;

namespace Guess_Word_Backend.Hubs.HubServices
{
    public class OnlinePlayersService
    {
        private readonly HubData _data;

        public OnlinePlayersService(HubData data)
        {
            _data = data;
        }

        public GetOnlinePlayersResponseDto GetOnlinePlayers(GetOnlinePlayersRequestDto requestDto)
        {
            var players = _data.Players.Skip(requestDto.PageNumber - 1 * requestDto.PageSize).Take(requestDto.PageSize).ToList();
            var total = _data.Players.Count;

            return new GetOnlinePlayersResponseDto(requestDto.PageNumber, requestDto.PageSize, total, players);
        }

        public PlayerDto AddPlayer(string id, string connectionId, string name)
        {
            id = id.Trim('"', '/', '\\');
            var player = _data.Players.Find(p => p.Id == id);
            if (player != null)
            {
                UpdatePlayer(player);
            }
            else {
                player = new PlayerDto(id, connectionId, name, 0, 0);

                _data.Players.Add(player);
            }
            return player;
        }
        public void UpdatePlayer(PlayerDto player)
        {
            var p = _data.Players.Find(p => p.Id == player.Id);
            if (p != null) {
                _data.Players[_data.Players.IndexOf(p)] = player;
                return;
            }
            Console.WriteLine("Failed to update the Player [Player does not exist]");
        }
        public PlayerDto? GetPlayer(Predicate<PlayerDto> expression)
        {
            return _data.Players.Find(expression);
        }
        public PlayerDto? GetPlayerId(string id)
        {
            return _data.Players.Find(p=> p.Id == id);
        }
        public PlayerDto? GetPlayerConnectionId(string connectionId)
        {
            return _data.Players.Find(p=> p.ConnectionId == connectionId);
        }
        public List<PlayerDto> GetPlayersRange(Predicate<PlayerDto> expression)
        {
            return _data.Players.FindAll(expression);
        }

        public void RemoveBy(string? userId, string connectionId)
        {
            var player = GetPlayer(p=>p.Id == userId || p.ConnectionId == connectionId);
            if (player != null)
            {
                _data.Players.Remove(player);
            }
        }
    }
}
