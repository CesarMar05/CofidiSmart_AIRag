using System;
using System.Linq.Expressions;

namespace AT2Soft.RAGEngine.Domain.Interfaces;

public interface IRepository<T,Y> where T : class
{
    Task<T?> GetByIdAsync(Y id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetListAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>>? predicate, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, bool asNoTracking = true, int? skip = null, int? take = null, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}