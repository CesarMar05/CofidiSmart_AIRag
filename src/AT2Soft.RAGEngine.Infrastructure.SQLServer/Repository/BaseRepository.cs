using System.Linq.Expressions;
using AT2Soft.RAGEngine.Domain.Interfaces;
using AT2Soft.RAGEngine.Infrastructure.SQLServer.Data;
using Microsoft.EntityFrameworkCore;

namespace AT2Soft.RAGEngine.Infrastructure.SQLServer.Repository;

// Infrastructure Layer
public abstract class BaseRepository<T, Y> : IRepository<T, Y> where T : class
{
    protected readonly RAGSqlServerDbContext _context;
    protected readonly DbSet<T> _dbSet;

    protected BaseRepository(RAGSqlServerDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Y id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        return (await _dbSet.AddAsync(entity, cancellationToken)).Entity;
    }

    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual async Task<IEnumerable<T>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _dbSet.ToArrayAsync(cancellationToken);
        return entities;
    }

    public virtual async Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>>? predicate, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet;

        if (predicate is not null)
            query = query.Where(predicate);

        return await query.ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool asNoTracking = true,
        int? skip = null,
        int? take = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        if (asNoTracking)
            query = query.AsNoTracking();

        if (includes is { Length: > 0 })
            foreach (var include in includes)
                query = query.Include(include);

        if (predicate is not null)
            query = query.Where(predicate);

        if (orderBy is not null)
            query = orderBy(query);

        if (skip is not null) query = query.Skip(skip.Value);
        if (take is not null) query = query.Take(take.Value);

        return await query.ToListAsync(cancellationToken);
    }
}