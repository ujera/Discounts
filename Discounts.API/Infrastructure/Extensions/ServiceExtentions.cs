// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Repositories;
using Discounts.Application.Interfaces.Services;
using Discounts.Application.Services;
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

            //services
            services.AddScoped<IReservationService, ReservationService>();
            services.AddScoped<IOfferService, OfferService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IMerchantService, MerchantService>();
            services.AddScoped<ISystemSettingService, SystemSettingService>();
        }
    }
}
