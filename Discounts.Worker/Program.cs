// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Repositories;
using Discounts.Application.Interfaces.Services;
using Discounts.Application.Services;
using Discounts.Infrastructure.Repositories;
using Discounts.Persistance.Context;
using Discounts.Worker;
using Microsoft.EntityFrameworkCore;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        services.AddDbContext<DiscountsManagementContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IOfferService, OfferService>();

        services.AddHostedService<SystemCleanupWorker>();
    })
    .Build();

await host.RunAsync();
