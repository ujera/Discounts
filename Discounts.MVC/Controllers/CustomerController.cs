using System.Security.Claims;
using Discounts.Application.Interfaces.Services;
using Discounts.MVC.Models.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.MVC.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly IReservationService _reservationService;

        public CustomerController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet]
        public async Task<IActionResult> MyCoupons(CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }
            var reservations = await _reservationService.GetCustomerReservationsAsync(userId, ct);
            var purchasedCoupons = await _reservationService.GetMyCouponsAsync(userId, ct);
            var viewModel = new MyCouponsViewModel
            {
                ActiveReservations = reservations,
                PurchasedCoupons = purchasedCoupons
            };
            return View(viewModel);
        }
    }
}
