using Discounts.Application.Exceptions;
using Discounts.Application.Services;
using Discounts.Application.Tests;
using Discounts.Domain.Entities;
using Moq;

namespace Discounts.UnitTests
{
    public class ReservationServiceTest : ServiceTestBase
    {
        private readonly ReservationService _service;

        public ReservationServiceTest()
        {
            _service = new ReservationService(MockUnitOfWork.Object);
        }

        [Fact]
        public async Task ReserveOfferAsync_WhenValid_ShouldDecreaseCouponCountAndCommit()
        {
            // Arrange
            var offerId = 1;
            var userId = "user-1";
            var offer = new Offer { Id = offerId, Title = "Pizza", CouponsCount = 10 };

            MockOfferRepo.Setup(r => r.GetByIdAsync(offerId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(offer);

            // Act
            var result = await _service.ReserveOfferAsync(offerId, userId, CancellationToken.None);

            // Assert
            Assert.Equal(9, offer.CouponsCount); // რაოდენობა უნდა შემცირდეს
            MockReservationRepo.Verify(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()), Times.Once);
            MockTransaction.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            MockUnitOfWork.Verify(u => u.SaveAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ReserveOfferAsync_WhenSoldOut_ShouldThrowSoldOutException()
        {
            // Arrange
            var offer = new Offer { Id = 1, CouponsCount = 0, Title = "No Pizza" };
            MockOfferRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(offer);

            // Act & Assert
            await Assert.ThrowsAsync<SoldOutException>(() =>
                _service.ReserveOfferAsync(1, "user-1", CancellationToken.None));
        }

        [Fact]
        public async Task CleanupExpiredReservationsAsync_ShouldIncreaseCouponCount()
        {
            // Arrange
            var offerId = 1;
            var expiredReservations = new List<Reservation>
            {
                new Reservation { Id = 10, OfferId = offerId }
            };
            var offer = new Offer { Id = offerId, CouponsCount = 5 };

            MockReservationRepo.Setup(r => r.GetExpiredReservationsAsync(It.IsAny<CancellationToken>()))
                              .ReturnsAsync(expiredReservations);
            MockOfferRepo.Setup(r => r.GetByIdAsync(offerId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(offer);

            // Act
            await _service.CleanupExpiredReservationsAsync(CancellationToken.None);

            // Assert
            Assert.Equal(6, offer.CouponsCount); // კუპონი უკან უნდა დაბრუნდეს
            MockReservationRepo.Verify(r => r.Remove(It.IsAny<Reservation>()), Times.Once);
            MockUnitOfWork.Verify(u => u.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ReserveOfferAsync_OnException_ShouldRollback()
        {
            // Arrange
            MockOfferRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(new Exception("DB Error"));

            // Act
            try { await _service.ReserveOfferAsync(1, "u", CancellationToken.None); } catch { }

            // Assert
            MockTransaction.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
