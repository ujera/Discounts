// Copyright (C) TBC Bank. All Rights Reserved.

using System.Linq.Expressions;

namespace Discounts.Application.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Remove(T entity);
        void Update(T entity);
    }
}
