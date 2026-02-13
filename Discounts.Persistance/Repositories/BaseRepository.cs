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

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id).ConfigureAwait(false);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync().ConfigureAwait(false);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity).ConfigureAwait(false);
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
