using Core.Entities;
using Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implementation.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {

        private readonly AppDbContext _db;
        private readonly DbSet<T> _entity;
        public BaseRepository(AppDbContext db) { _db = db; _entity = _db.Set<T>(); }

        public async Task AddAsync(T entity, CancellationToken ct = default)
        {
            await _entity.AddAsync(entity, ct);
            //await SaveChangesAsync(ct);
            await _db.SaveChangesAsync(ct);

        }

        public async Task<T?> GetAsync(Guid id,List<Expression<Func<T,object>>>? incldes = default, CancellationToken ct = default)
        {
            incldes ??= [];
            foreach (var i in incldes)
            {
                _entity.Include(i);
            }
            return await _entity.FindAsync(id, ct).ConfigureAwait(false);
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> expression, List<Expression<Func<T, object>>>? incldes = default, CancellationToken ct = default)
        {
            incldes ??= [];
            var query = _entity.AsQueryable();
            foreach (var i in incldes)
            {
                 query = query.Include(i);
            }
            return await query.FirstOrDefaultAsync(expression, ct);
        }

        public async Task UpdateAsync(T entity, CancellationToken ct = default)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _entity.Update(entity);

            //await SaveChangesAsync(ct);
            await _db.SaveChangesAsync(ct);
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

        

        public async Task<ICollection<T>> GetRangeAsync(Expression<Func<T, bool>> expression, List<Expression<Func<T, object>>>? incldes = default, CancellationToken ct = default)
        {
            incldes ??= [];
            foreach (var i in incldes)
            {
                _entity.Include(i);
            }
            return await _entity.Where(expression).ToListAsync(ct);

        }

        public async Task DeleteAsync(T entity, CancellationToken ct = default)
        {
            _db.Remove(entity);
            await SaveChangesAsync(ct);
        }

        public async Task DeleteSoftAsync(T entity, CancellationToken ct = default)
        {
           entity.IsDeleted = true;
           await UpdateAsync(entity, ct);
        }

        public async Task<PagedList<T>> GetPagedAsync(PagedListRequestParameters parameters, Expression<Func<T, bool>>? expression = null, List<Expression<Func<T, object>>>? incldes = null, CancellationToken ct = default)
        {
            var query = _entity.AsNoTracking();
            if (expression is not null)
            {
                query = _entity.Where(expression);
            }
            incldes ??= [];
            foreach (var include in incldes)
            {
                query = query.Include(include);
            }
            query = query.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize).OrderBy(e => e.UpdatedAt);
            int totalCount = query.Count();
            var items = await query.ToListAsync(ct);
            return new PagedList<T>(parameters.PageNumber,parameters.PageSize,totalCount, items ?? []);
        }
    }
}
