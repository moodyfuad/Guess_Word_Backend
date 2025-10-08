using WordleServer.Models;

namespace WordleServer.Repositories
{
    public interface IGameRepository
    {
        Task<Game?> GetByKeyAsync(string gameKey, CancellationToken ct = default);
        Task AddAsync(Game game, CancellationToken ct = default);
        Task UpdateAsync(Game game, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
