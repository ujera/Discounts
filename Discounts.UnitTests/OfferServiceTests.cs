using System.Linq.Expressions;
using Discounts.Application.DTOs.Offer;
using Discounts.Application.Exceptions;
using Discounts.Application.Services;
using Discounts.Application.Tests;
using Discounts.Domain.Entities;
using Discounts.Domain.Enums;
using Moq;

namespace Discounts.UnitTests
{
    public class OfferServiceTest : ServiceTestBase
    {
        private readonly OfferService _service;

        public OfferServiceTest()
        {
            _service = new OfferService(MockUnitOfWork.Object);
        }


        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WhenValidData_ShouldReturnOfferDto()
        {
            // Arrange
            var merchantId = "123";
            var dto = new CreateOfferDto { CategoryId = 1, Title = "Summer Sale" };
            var category = new Category { Id = 1, Name = "Fashion" };

            MockCategoryRepo.Setup(r => r.GetByIdAsync(dto.CategoryId, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(category);

            // Act
            var result = await _service.CreateAsync(dto, merchantId, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Title, result.Title);

            MockOfferRepo.Verify(r => r.AddAsync(It.Is<Offer>(o => o.MerchantId == merchantId), It.IsAny<CancellationToken>()), Times.Once);
            MockUnitOfWork.Verify(u => u.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenCategoryNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            MockCategoryRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                            .ReturnsAsync((Category)null!);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.CreateAsync(new CreateOfferDto { CategoryId = 99 }, "m-1", CancellationToken.None));
        }

        #endregion

        #region Cleanup Logic Tests

        [Fact]
        public async Task CleanupExpiredOffersAsync_WhenOffersExpired_ShouldSetStatusToExpired()
        {
            // Arrange
            var expiredOffers = new List<Offer>
            {
                new Offer { Id = 1, Status = OfferStatus.Active, EndDate = DateTime.UtcNow.AddDays(-1) },
                new Offer { Id = 2, Status = OfferStatus.Active, EndDate = DateTime.UtcNow.AddDays(-2) }
            };

            MockOfferRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Offer, bool>>>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expiredOffers);

            // Act
            await _service.CleanupExpiredOffersAsync(CancellationToken.None);

            // Assert
            Assert.All(expiredOffers, o => Assert.Equal(OfferStatus.Expired, o.Status));

            MockOfferRepo.Verify(r => r.UpdateRange(expiredOffers), Times.Once);
            MockUnitOfWork.Verify(u => u.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region Update Logic Tests

        [Fact]
        public async Task UpdateAsync_WhenUserIsNotOwner_ShouldThrowUnauthorized()
        {
            // Arrange
            var offerId = 1;
            var realMerchantId = "owner";
            var hackerId = "hacker";
            var existingOffer = new Offer { Id = offerId, MerchantId = realMerchantId };

            MockOfferRepo.Setup(r => r.GetByIdAsync(offerId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(existingOffer);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.UpdateAsync(offerId, new UpdateOfferDto(), hackerId, CancellationToken.None));
        }

        #endregion
    }
}

