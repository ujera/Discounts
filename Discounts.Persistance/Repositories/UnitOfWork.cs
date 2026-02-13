// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Repositories;
using Discounts.Persistance.Context;

namespace Discounts.Persistance.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DiscountsManagementContext _context;

        public IOfferRepository Offers { get; private set; }
        public ICouponRepository Coupons { get; private set; }
        public ICategoryRepository Categories { get; private set; }
        public IReservationRepository Reservations { get; private set; }
        public ISystemSettingRepository Settings { get; private set; }

        public UnitOfWork(DiscountsManagementContext context)
        {
            _context = context;

            //To use same Context and both happen in same transaction
            Offers = new OfferRepository(_context);
            Coupons = new CouponRepository(_context);
            Categories = new CategoryRepository(_context);
            Reservations = new ReservationRepository(_context);
            Settings = new SystemSettingRepository(_context);
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
