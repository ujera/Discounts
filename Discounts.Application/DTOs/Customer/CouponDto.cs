
namespace Discounts.Application.DTOs.Customer
{
    public class CouponDto
    {
        public string Code { get; set; } = string.Empty;
        public string OfferTitle { get; set; } = string.Empty;
        public decimal PricePaid { get; set; }
        public DateTime SoldAt { get; set; }
        public bool IsUsed { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
