namespace Discounts.Domain.Entities
{
    public class Coupon
    {
        public int Id { get; set; }
        public string Code { get; set; }

        public DateTime SoldAt { get; set; } = DateTime.UtcNow;
        public bool IsUsed { get; set; }  // "Used" means scanned at the merchant

        // Relationships
        public string CustomerId { get; set; }
        public ApplicationUser Customer { get; set; }

        public int OfferId { get; set; }
        public Offer Offer { get; set; }
    }
}
