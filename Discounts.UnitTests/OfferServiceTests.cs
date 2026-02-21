using System.Linq.Expressions;
using Discounts.Application.DTOs.Offer;
using Discounts.Application.Exceptions;
using Discounts.Application.Pagination;
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

        [Fact]
        public async Task UpdateAsync_WhenUserIsOwner_ShouldUpdateAndSave()
        {
            // Arrange
            var offerId = 1;
            var merchantId = "owner";
            var existingOffer = new Offer { Id = offerId, MerchantId = merchantId, Title = "Old Title" };
            var updateDto = new UpdateOfferDto { Title = "New Title", CategoryId = 2 };
            var category = new Category { Id = 2, Name = "Electronics" };

            MockOfferRepo.Setup(r => r.GetByIdAsync(offerId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(existingOffer);
            MockCategoryRepo.Setup(r => r.GetByIdAsync(updateDto.CategoryId, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(category);

            // Act
            await _service.UpdateAsync(offerId, updateDto, merchantId, CancellationToken.None);

            // Assert
            Assert.Equal("New Title", existingOffer.Title);
            MockUnitOfWork.Verify(u => u.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region Filtering Logic Tests
        [Fact]
        public async Task GetActiveOffersAsync_ShouldReturnOnlyActiveAndNonExpiredOffers()
        {
            // Arrange
            var activeOffers = new List<Offer>
            {
                new Offer { Id = 1, Status = OfferStatus.Active, EndDate = DateTime.UtcNow.AddDays(1) }
            };

            MockOfferRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Offer, bool>>>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(activeOffers);

            // Act
            var result = await _service.GetActiveOffersAsync(CancellationToken.None);

            // Assert
            Assert.Single(result);
            MockOfferRepo.Verify(r => r.FindAsync(It.IsAny<Expression<Func<Offer, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetPendingOffersAsync_ShouldReturnOnlyPendingOffers()
        {
            // Arrange
            var pendingOffers = new List<Offer>
            {
                new Offer { Id = 1, Status = OfferStatus.Pending }
            };

            MockOfferRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Offer, bool>>>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(pendingOffers);

            // Act
            var result = await _service.GetPendingOffersAsync(CancellationToken.None);

            // Assert
            Assert.Single(result);
            MockOfferRepo.Verify(r => r.FindAsync(It.IsAny<Expression<Func<Offer, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        #endregion

        #region Pagination Tests
        [Theory]
        [InlineData("iPhone", null, null, 1)] //  ძებნა
        [InlineData(null, 1, null, 1)]      //  კატეგორია
        public async Task GetPagedOffersAsync_WithVariousFilters_ShouldReturnFilteredResults(
            string searchTerm, int? catId, decimal? maxPrice, int expectedCount)
        {
            // Arrange
            var filter = new OfferFilterDto 
            { 
                SearchTerm = searchTerm, 
                CategoryId = catId, 
                MaxPrice = maxPrice,
                PageNumber = 1,
                PageSize = 10 
            };

            var offers = new List<Offer> 
            { 
                new Offer { Id = 1, Title = "iPhone 15", CategoryId = 1, DiscountPrice = 1200, Status = OfferStatus.Active, EndDate = DateTime.UtcNow.AddDays(1) },
                new Offer { Id = 2, Title = "Pizza", CategoryId = 2, DiscountPrice = 20, Status = OfferStatus.Active, EndDate = DateTime.UtcNow.AddDays(1) }
            };

            var pagedResult = new PagedResult<Offer>(offers.Take(expectedCount).ToList(), expectedCount, 1, 10);
            
            MockOfferRepo.Setup(r => r.GetPagedOffersAsync(It.IsAny<OfferFilterDto>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(pagedResult);

            // Act
            var result = await _service.GetAllActiveAsync(filter, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            MockOfferRepo.Verify(r => r.GetPagedOffersAsync(filter, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetPagedOffersAsync_ShouldReturnCorrectPaginationMetadata()
        {
            // Arrange
            var filter = new OfferFilterDto { PageNumber = 2, PageSize = 5 };
            var totalItems = 20;
            var items = new List<Offer> { new Offer { Id = 6 }, new Offer { Id = 7 } };

            var pagedResult = new PagedResult<Offer>(items, totalItems, filter.PageNumber, filter.PageSize);

            MockOfferRepo.Setup(r => r.GetPagedOffersAsync(filter, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(pagedResult);

            // Act
            var result = await _service.GetAllActiveAsync(filter, CancellationToken.None);

            // Assert
            Assert.Equal(2, result.PageNumber);
            Assert.Equal(4, result.TotalPages);
            Assert.Equal(totalItems, result.TotalCount);
        }

        [Fact]
        public async Task GetPagedOffersAsync_WhenNoMatchesFound_ShouldReturnEmptyPagedResult()
        {
            // Arrange
            var filter = new OfferFilterDto { SearchTerm = "NonExistentOffer" };
            var emptyResult = new PagedResult<Offer>(new List<Offer>(), 0, 1, 10);

            MockOfferRepo.Setup(r => r.GetPagedOffersAsync(It.IsAny<OfferFilterDto>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(emptyResult);

            // Act
            var result = await _service.GetAllActiveAsync(filter, CancellationToken.None);

            // Assert
            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }
        #endregion
    }
}

