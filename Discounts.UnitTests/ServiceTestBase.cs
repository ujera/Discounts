using AutoMapper;
using Discounts.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace Discounts.Application.Tests
{
    public abstract class ServiceTestBase
    {
        protected readonly Mock<IUnitOfWork> MockUnitOfWork;
        protected readonly Mock<IMapper> MockMapper;

        protected readonly Mock<IOfferRepository> MockOfferRepo = new();
        protected readonly Mock<ICategoryRepository> MockCategoryRepo = new();
        protected readonly Mock<ICouponRepository> MockCouponRepo = new();
        protected readonly Mock<IReservationRepository> MockReservationRepo = new();
        protected readonly Mock<ISystemSettingRepository> MockSettingRepo = new();
        protected readonly Mock<IDbContextTransaction> MockTransaction = new();

        protected ServiceTestBase()
        {
            MockUnitOfWork = new Mock<IUnitOfWork>();
            MockMapper = new Mock<IMapper>();

            MockUnitOfWork.Setup(u => u.Offers).Returns(MockOfferRepo.Object);
            MockUnitOfWork.Setup(u => u.Categories).Returns(MockCategoryRepo.Object);
            MockUnitOfWork.Setup(u => u.Coupons).Returns(MockCouponRepo.Object);
            MockUnitOfWork.Setup(u => u.Reservations).Returns(MockReservationRepo.Object);
            MockUnitOfWork.Setup(u => u.Settings).Returns(MockSettingRepo.Object);
            MockUnitOfWork.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync(MockTransaction.Object);

            MockUnitOfWork.Setup(u => u.SaveAsync(It.IsAny<CancellationToken>()))
                         .ReturnsAsync(1);
        }
    }
}
