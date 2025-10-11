using Guess_Word_Backend.Models;
using System.Linq.Expressions;

namespace Guess_Word_Backend.Repositories
{
    public interface IBaseRepository<T>
    {
        Task<T?> GetAsync(Guid id, CancellationToken ct = default);
        Task<T?> GetAsync(Expression<Func<T,bool>> expression, CancellationToken ct = default);
        Task<ICollection<T>> GetRangeAsync(Expression<Func<T,bool>> expression, CancellationToken ct = default);
        Task AddAsync(T entity, CancellationToken ct = default);
        Task UpdateAsync(T entity, CancellationToken ct = default);
        Task DeleteAsync(T entity, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
