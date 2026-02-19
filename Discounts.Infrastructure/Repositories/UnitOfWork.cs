// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Repositories;
using Discounts.Persistance.Context;
using Discounts.Persistance.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Discounts.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DiscountsManagementContext _context;

        private IOfferRepository? _offers;
        private ICouponRepository? _coupons;
        private ICategoryRepository? _categories;
        private IReservationRepository? _reservations;
        private ISystemSettingRepository? _settings;

        public UnitOfWork(DiscountsManagementContext context)
        {
            _context = context;
        }

        //I inject the scoped DbContext via DI, and use Lazy Initialization
        //passing them the shared context to ensure transaction integrity
        public IOfferRepository Offers => _offers ??= new OfferRepository(_context);
        public ICouponRepository Coupons => _coupons ??= new CouponRepository(_context);
        public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);
        public IReservationRepository Reservations => _reservations ??= new ReservationRepository(_context);
        public ISystemSettingRepository Settings => _settings ??= new SystemSettingRepository(_context);

        public async Task<int> SaveAsync(CancellationToken ct)
        {
            return await _context.SaveChangesAsync(ct).ConfigureAwait(false);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
        {
            return await _context.Database.BeginTransactionAsync(ct).ConfigureAwait(false);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
