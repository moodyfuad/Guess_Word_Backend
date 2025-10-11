using Guess_Word_Backend.Models;

namespace Guess_Word_Backend.Repositories
{
    public interface IGameRoomRepository : IBaseRepository<GameRoom>
    {
        Task<GameRoom?> GetByKey(string key, CancellationToken ct); 
        //Task<GameRoom?> Create(GameRoom gameRoom, CancellationToken ct); 
        //Task<GameRoom?> upda(GameRoom gameRoom, CancellationToken ct); 
    }
}
