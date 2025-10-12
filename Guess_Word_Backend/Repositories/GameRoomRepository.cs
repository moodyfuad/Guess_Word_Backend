using Guess_Word_Backend.Data;
using Guess_Word_Backend.Models;

namespace Guess_Word_Backend.Repositories
{
    public class GameRoomRepository : BaseRepository<GameRoom>, IGameRoomRepository
    {
        public GameRoomRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<GameRoom?> GetByKey(string key, CancellationToken ct)
        {
             return await base.GetAsync(g => g.Key.ToLower().Equals(key.ToLower()), ct);
        }
    }
}
