// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Repositories;
using Discounts.Persistance.Repositories;

namespace Discounts.API.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            //repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IOfferRepository, OfferRepository>();
            services.AddScoped<ICouponRepository, CouponRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<ISystemSettingRepository, SystemSettingRepository>();
        }
    }
}
