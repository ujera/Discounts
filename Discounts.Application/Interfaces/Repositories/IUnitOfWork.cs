namespace Discounts.Application.Interfaces.Repositories
{

    public interface IUnitOfWork : IDisposable
    {
        IOfferRepository Offers { get; }
        ICouponRepository Coupons { get; }
        ICategoryRepository Categories { get; }
        IReservationRepository Reservations { get; }
        ISystemSettingRepository Settings { get; }

        Task<int> SaveAsync(CancellationToken ct);
    }

}
