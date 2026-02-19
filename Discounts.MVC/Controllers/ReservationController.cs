using System.Security.Claims;
using Discounts.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.MVC.Controllers
{
    [Authorize(Roles = "Customer")]
    public class ReservationController : Controller
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpPost]
        public async Task<IActionResult> Reserve(int offerId, CancellationToken ct)
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(customerId))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                await _reservationService.ReserveOfferAsync(offerId, customerId, ct);

                TempData["SuccessMessage"] = "კუპონი წარმატებით დაიჯავშნა! დრო შეზღუდულია, გთხოვთ დროულად შეიძინოთ.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Details", "Offer", new { id = offerId });
        }
    }
}
