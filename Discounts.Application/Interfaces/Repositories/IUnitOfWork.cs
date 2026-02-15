using Microsoft.EntityFrameworkCore.Storage;

namespace Discounts.Application.Interfaces.Repositories
{

    public interface IUnitOfWork : IDisposable
    {
        IOfferRepository Offers { get; }
        ICouponRepository Coupons { get; }
        ICategoryRepository Categories { get; }
        IReservationRepository Reservations { get; }
        ISystemSettingRepository Settings { get; }

        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
        Task<int> SaveAsync(CancellationToken ct);
    }

}
