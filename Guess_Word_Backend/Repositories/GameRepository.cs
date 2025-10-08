using Microsoft.EntityFrameworkCore;
using WordleServer.Data;
using WordleServer.Models;

namespace WordleServer.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly AppDbContext _db;
        public GameRepository(AppDbContext db) { _db = db; }

        public async Task AddAsync(Game game, CancellationToken ct = default)
        {
            await _db.Games.AddAsync(game, ct);
        }

        public async Task<Game?> GetByKeyAsync(string gameKey, CancellationToken ct = default)
        {
            return await _db.Games
                .Include(g => g.Players)
                .Include(g => g.Guesses.OrderBy(x => x.CreatedAt))
                .FirstOrDefaultAsync(g => g.GameKey == gameKey, ct);
        }

        public Task UpdateAsync(Game game, CancellationToken ct = default)
        {
            _db.Games.Update(game);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken ct = default) =>
            _db.SaveChangesAsync(ct);
    }
}
