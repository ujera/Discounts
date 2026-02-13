using Microsoft.AspNetCore.Identity;

namespace Discounts.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public ICollection<Offer> Offers { get; set; } = new List<Offer>();// Offers by merchant
        public ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();// Coupons by costumer
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>(); //costumer
    }
}
