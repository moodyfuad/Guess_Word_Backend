using Core.Entities;
using Shared.Helpers;
using System.Linq.Expressions;

namespace Core.Repositories
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<T?> GetAsync(Guid id, List<Expression<Func<T, object>>>? incldes = default, CancellationToken ct = default);
        Task<PagedList<T>> GetPagedAsync(PagedListRequestParameters parameters, Expression<Func<T, bool>>? expression = default, List<Expression<Func<T, object>>>? incldes = default, CancellationToken ct = default);
        Task<T?> GetAsync(Expression<Func<T,bool>> expression, List<Expression<Func<T, object>>>? incldes = default, CancellationToken ct = default);
        Task<ICollection<T>> GetRangeAsync(Expression<Func<T,bool>> expression, List<Expression<Func<T, object>>>? incldes = default, CancellationToken ct = default);
        Task AddAsync(T entity, CancellationToken ct = default);
        Task UpdateAsync(T entity, CancellationToken ct = default);
        Task DeleteAsync(T entity, CancellationToken ct = default);
        Task DeleteSoftAsync(T entity, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
