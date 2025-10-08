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
            await SaveChangesAsync(ct);
        }

        public async Task<Game?> GetByKeyAsync(string gameKey, CancellationToken ct = default)
        {
            return await _db.Games.AsTracking()
                .Include(g => g.Players)
                .Include(g => g.Guesses.OrderBy(x => x.CreatedAt))
                .FirstOrDefaultAsync(g => g.GameKey == gameKey, ct).ConfigureAwait(false);
        }

        public Task UpdateAsync(Game game, CancellationToken ct = default)
        {
            _db.Games.Update(game);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            foreach (var entry in _db.ChangeTracker.Entries())
            {
                Console.WriteLine($"Tracked: {entry.Entity.GetType().Name}, State: {entry.State}");
            }

            try
            {
                await _db.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Log details for debugging
                Console.WriteLine($"Concurrency issue: {ex.Message}");
                foreach (var entry in ex.Entries)
                {
                    Console.WriteLine($"Entity {entry.Entity.GetType().Name} caused concurrency issue.");
                }

                // Reload the entity from DB to resolve stale state
                foreach (var entry in ex.Entries)
                {
                    await entry.ReloadAsync(ct);
                }
            }
        }

    }
}
