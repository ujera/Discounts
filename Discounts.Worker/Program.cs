// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Repositories;
using Discounts.Application.Interfaces.Services;
using Discounts.Application.Services;
using Discounts.Persistance.Context;
using Discounts.Persistance.Repositories;
using Discounts.Worker;
using Microsoft.EntityFrameworkCore;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // 1. CONFIGURATION (Access appsettings.json)
        IConfiguration configuration = hostContext.Configuration;

    // 2. DATABASE (Must match API config) [cite: 39]
        services.AddDbContext<DiscountsManagementContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

      // 3. REPOSITORIES & UNIT OF WORK [cite: 54]
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // 4. SERVICES (Logic)
        // We only need the services required for cleanup
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IOfferService, OfferService>();

        // Note: We don't need AuthService or JWT here! The worker is internal.

// 5. REGISTER THE WORKER 
        services.AddHostedService<SystemCleanupWorker>();
    })
    .Build();

await host.RunAsync();
