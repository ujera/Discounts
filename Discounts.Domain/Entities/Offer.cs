using Discounts.Domain.Enums;

namespace Discounts.Domain.Entities
{
    public class Offer
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        public decimal OriginalPrice { get; set; }
        public decimal DiscountPrice { get; set; }

        public int CouponsCount { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public OfferStatus Status { get; set; } = OfferStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? RejectionReason { get; set; }

        // Foreign Keys
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public string MerchantId { get; set; }
        public ApplicationUser Merchant { get; set; }

        public ICollection<Coupon> Coupons { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}
