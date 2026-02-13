namespace Discounts.Domain.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public DateTime ReservedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }

        // foreign keys
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int OfferId { get; set; }
        public Offer Offer { get; set; }
    }
}
