using Guess_Word_Backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Guess_Word_Backend.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly AppDbContext _db;
        private readonly DbSet<T> _entity;
        public BaseRepository(AppDbContext db) { _db = db;_entity = _db.Set<T>(); }

        public async Task AddAsync(T entity, CancellationToken ct = default)
        {
            await _entity.AddAsync(entity, ct);
            await SaveChangesAsync(ct);
        }

        public async Task<T?> GetAsync(Guid id, CancellationToken ct = default)
        {
            return await _entity.FindAsync(id, ct).ConfigureAwait(false);
        }

        public async Task UpdateAsync(T entity, CancellationToken ct = default)
        {
            _entity.Update(entity);
            await SaveChangesAsync(ct);
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

        public async Task<T?> GetAsync(Expression<Func<T, bool>> expression, CancellationToken ct = default)
        {
            return await _entity.FirstOrDefaultAsync(expression,ct);
        }

        public async Task<ICollection<T>> GetRangeAsync(Expression<Func<T, bool>> expression, CancellationToken ct = default)
        {
            return await _entity.Where(expression).ToListAsync();
            
        }

        public async Task DeleteAsync(T entity, CancellationToken ct = default)
        {
            _db.Remove(entity);
            await SaveChangesAsync(ct);
        }
    }
}
