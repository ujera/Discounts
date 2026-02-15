// Copyright (C) TBC Bank. All Rights Reserved.

using System.Linq.Expressions;

namespace Discounts.Application.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id, CancellationToken ct);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken ct);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct);
        Task AddAsync(T entity, CancellationToken ct);
        void Remove(T entity);
        void Update(T entity);
    }
}
