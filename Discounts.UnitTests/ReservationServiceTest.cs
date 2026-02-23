using Discounts.Application.DTOs.Customer;
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

        #region PurchaseReservationAsync Tests

        [Fact]
        public async Task PurchaseReservationAsync_WhenReservationNotFound_ShouldThrowNotFound()
        {
            // Arrange
            MockReservationRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                               .ReturnsAsync((Reservation?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.PurchaseReservationAsync(1, "user-1", CancellationToken.None));
        }

        [Fact]
        public async Task PurchaseReservationAsync_WhenReservationExpired_ShouldThrowBadRequest()
        {
            // Arrange
            var reservation = new Reservation
            {
                Id = 1,
                UserId = "user-1",
                ExpiresAt = DateTime.UtcNow.AddMinutes(-31) // Expired  30min limit
            };

            MockReservationRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(reservation);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.PurchaseReservationAsync(1, "user-1", CancellationToken.None));

            Assert.Contains("expired", ex.Message.ToLower());
        }

        [Fact]
        public async Task PurchaseReservationAsync_WhenValid_ShouldCreateCouponAndCompleteTransaction()
        {
            // Arrange
            var userId = "user-1";
            var reservation = new Reservation
            {
                Id = 1,
                UserId = userId,
                OfferId = 10,
                ReservedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(31)
            };

            // Setup offer
            var offer = new Offer { Id = 10, Title = "Sushi Deal" };
            MockOfferRepo.Setup(o => o.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(offer);

            MockReservationRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(reservation);

            // Act
            var result = await _service.PurchaseReservationAsync(1, userId, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Code);

            // Verify
            MockTransaction.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            MockCouponRepo.Verify(c => c.AddAsync(It.IsAny<Coupon>(), It.IsAny<CancellationToken>()), Times.Once);
            MockUnitOfWork.Verify(u => u.SaveAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task PurchaseReservationAsync_OnDatabaseFailure_ShouldRollbackTransaction()
        {
            // Arrange
            var userId = "user-1";
            var reservation = new Reservation { Id = 1, UserId = userId, OfferId = 10, ReservedAt = DateTime.UtcNow, ExpiresAt = DateTime.UtcNow.AddMinutes(31) };

            MockReservationRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(reservation);

            // Simulate
            MockCouponRepo.Setup(c => c.AddAsync(It.IsAny<Coupon>(), It.IsAny<CancellationToken>()))
                          .ThrowsAsync(new Exception("Database connection lost"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _service.PurchaseReservationAsync(1, userId, CancellationToken.None));

            // Verify Rollback happened
            MockTransaction.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
            MockTransaction.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion

        [Theory]
        [InlineData(29, false)] // (Success)
        [InlineData(31, true)]  //  (Expired)
        public async Task PurchaseReservation_TimeBoundaries_ShouldValidateExpiry(int minutesAgo, bool shouldFail)
        {
            // Arrange
            var userId = "user-1";
            var offerId = 10;
            var reservedAt = DateTime.UtcNow.AddMinutes(-minutesAgo);
            var reservation = new Reservation
            {
                Id = 1,
                UserId = userId,
                OfferId = offerId,
                ReservedAt = reservedAt,
                ExpiresAt = reservedAt.AddMinutes(30)
            };

            MockReservationRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(reservation);

            if (!shouldFail)
            {
                MockOfferRepo.Setup(o => o.GetByIdAsync(offerId, It.IsAny<CancellationToken>()))
                             .ReturnsAsync(new Offer { Id = offerId, Title = "Test Offer", DiscountPrice = 50 });

                MockCouponRepo.Setup(c => c.AddAsync(It.IsAny<Coupon>(), It.IsAny<CancellationToken>()))
                              .Returns(Task.CompletedTask);

                MockMapper.Setup(m => m.Map<CouponDto>(It.IsAny<Coupon>()))
                          .Returns(new CouponDto { Code = "SUCCESS-CODE" });
            }

            // Act & Assert
            if (shouldFail)
            {
                await Assert.ThrowsAsync<BadRequestException>(() =>
                    _service.PurchaseReservationAsync(1, userId, CancellationToken.None));
            }
            else
            {
                var result = await _service.PurchaseReservationAsync(1, userId, CancellationToken.None);
                Assert.NotNull(result);
            }
        }
    }
}
