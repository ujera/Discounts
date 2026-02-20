using Discounts.Application.DTOs.Customer;

namespace Discounts.MVC.Models.Customer
{
    public class MyCouponsViewModel
    {
        public IEnumerable<ReservationResponseDto> ActiveReservations { get; set; } = new List<ReservationResponseDto>();
        public IEnumerable<CouponDto> PurchasedCoupons { get; set; } = new List<CouponDto>();
    }
}
