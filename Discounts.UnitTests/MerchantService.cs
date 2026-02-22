using Discounts.Application.Services;
using Discounts.Application.Tests;
using Discounts.Domain.Entities;
using Discounts.Domain.Enums;
using Moq;
using System.Linq.Expressions;

namespace Discounts.UnitTests
{
    public class MerchantServiceTest : ServiceTestBase
    {
        private readonly MerchantService _service;

        public MerchantServiceTest()
        {
            _service = new MerchantService(MockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetDashboardStatsAsync_ShouldCalculateCorrectStats()
        {
            // Arrange
            var merchantId = "m-1";
            var offers = new List<Offer>
            {
                new Offer { MerchantId = merchantId, Status = OfferStatus.Active },
                new Offer { MerchantId = merchantId, Status = OfferStatus.Pending },
                new Offer { MerchantId = merchantId, Status = OfferStatus.Active }
            };

            var coupons = new List<Coupon>
            {
                new Coupon { Offer = new Offer { DiscountPrice = 10 } },
                new Coupon { Offer = new Offer { DiscountPrice = 20 } }
            };

            // მერჩანტის შეთავაზებების მოკირება
            MockOfferRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Offer, bool>>>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(offers);

            // გაყიდული კუპონების მოკირება
            MockCouponRepo.Setup(r => r.GetByMerchantIdAsync(merchantId, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(coupons);

            // Act
            var stats = await _service.GetDashboardStatsAsync(merchantId, CancellationToken.None);

            // Assert
            Assert.Equal(3, stats.TotalOffers);
            Assert.Equal(2, stats.ActiveOffers);
            Assert.Equal(2, stats.TotalCouponsSold);
            Assert.Equal(30, stats.TotalRevenue);
        }

        [Fact]
        public async Task GetSalesHistoryAsync_ShouldReturnMappedDtos()
        {
            // Arrange
            var merchantId = "m-1";
            var coupons = new List<Coupon>
            {
                new Coupon { Id = 1, Code = "C1", Offer = new Offer { Title = "Offer 1" } }
            };

            MockCouponRepo.Setup(r => r.GetByMerchantIdAsync(merchantId, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(coupons);

            // Act
            var result = await _service.GetSalesHistoryAsync(merchantId, CancellationToken.None);

            // Assert
            Assert.Single(result);
            MockCouponRepo.Verify(r => r.GetByMerchantIdAsync(merchantId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
