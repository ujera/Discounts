// Copyright (C) TBC Bank. All Rights Reserved.

using System.Linq.Expressions;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Persistance.Context;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Persistance.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly DiscountsManagementContext _context;
        internal DbSet<T> _dbSet;

        public BaseRepository(DiscountsManagementContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _dbSet.FindAsync(new object?[] { id, ct }, cancellationToken: ct).ConfigureAwait(false);
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct)
        {
            return await _dbSet.ToListAsync(ct).ConfigureAwait(false);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct)
        {
            return await _dbSet.Where(predicate).ToListAsync(ct).ConfigureAwait(false);
        }

        public async Task AddAsync(T entity, CancellationToken ct)
        {
            await _dbSet.AddAsync(entity,ct).ConfigureAwait(false);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}
