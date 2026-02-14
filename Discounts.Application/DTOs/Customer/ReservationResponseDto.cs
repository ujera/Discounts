namespace Discounts.Application.DTOs.Customer
{
    public class ReservationResponseDto
    {
        public int Id { get; set; }
        public int OfferId { get; set; }
        public string OfferTitle { get; set; } = string.Empty;
        public DateTime ReservedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
