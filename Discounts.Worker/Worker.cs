// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Services;

namespace Discounts.Worker
{
    public class SystemCleanupWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SystemCleanupWorker> _logger;

        public SystemCleanupWorker(IServiceProvider serviceProvider, ILogger<SystemCleanupWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();

                    var reservationService = scope.ServiceProvider.GetRequiredService<IReservationService>();
                    var offerService = scope.ServiceProvider.GetRequiredService<IOfferService>();

                    await reservationService.CleanupExpiredReservationsAsync(stoppingToken).ConfigureAwait(false);
                    await offerService.CleanupExpiredOffersAsync(stoppingToken).ConfigureAwait(false);

                    _logger.LogInformation("Cleanup cycle finished at: {time}", DateTimeOffset.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Worker failed to execute cleanup.");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken).ConfigureAwait(false);
            }
        }
    }
}
