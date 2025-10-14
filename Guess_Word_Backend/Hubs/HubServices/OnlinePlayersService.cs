using Guess_Word_Backend.Hubs.HubDtos;
using Guess_Word_Backend.Hubs.Repositories;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Linq.Expressions;

namespace Guess_Word_Backend.Hubs.HubServices
{
    public class OnlinePlayersService
    {

        public static GetOnlinePlayersResponseDto GetOnlinePlayers(GetOnlinePlayersRequestDto requestDto)
        {
            var players = HubData.Players.Skip(requestDto.PageNumber - 1 * requestDto.PageSize).Take(requestDto.PageSize).ToList();
            var total = Repositories.HubData.Players.Count;

            return new GetOnlinePlayersResponseDto(requestDto.PageNumber, requestDto.PageSize, total, players);
        }

        public static PlayerDto AddPlayer(string id, string connectionId, string name)
        {
            id = id.Trim('"', '/', '\\');
            var player = Repositories.HubData.Players.Find(p => p.Id == id);
            if (player != null)
            {
                UpdatePlayer(player);
            }
            else {
                player = new PlayerDto(id, connectionId, name, 0, 0);

                Repositories.HubData.Players.Add(player);
            }
            return player;
        }
        public static void UpdatePlayer(PlayerDto player)
        {
            var p = Repositories.HubData.Players.Find(p => p.Id == player.Id);
            if (p != null) {
                Repositories.HubData.Players[Repositories.HubData.Players.IndexOf(p)] = player;
                return;
            }
            Console.WriteLine("Failed to update the Player [Player does not exist]");
        }
        public static PlayerDto? GetPlayer(Predicate<PlayerDto> expression)
        {
            return Repositories.HubData.Players.Find(expression);
        }
        public static PlayerDto? GetPlayerId(string id)
        {
            return Repositories.HubData.Players.Find(p=> p.Id == id);
        }
        public static PlayerDto? GetPlayerConnectionId(string connectionId)
        {
            return Repositories.HubData.Players.Find(p=> p.ConnectionId == connectionId);
        }
        public static List<PlayerDto> GetPlayersRange(Predicate<PlayerDto> expression)
        {
            return Repositories.HubData.Players.FindAll(expression);
        }

        public static void RemoveBy(string? userId, string connectionId)
        {
            var player = GetPlayer(p=>p.Id == userId || p.ConnectionId == connectionId);
            if (player != null)
            {
                Repositories.HubData.Players.Remove(player);
            }
        }
    }
}
