using System.Security.Claims;
using Discounts.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.MVC.Controllers
{
    [Authorize(Roles = "Customer")]
    public class PurchaseController : Controller
    {
        private readonly IReservationService _reservationService;

        public PurchaseController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpPost]
        public async Task<IActionResult> Buy(int? offerId, int? reservationId, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Auth");

            try
            {
                if (reservationId.HasValue)
                {
                    await _reservationService.PurchaseReservationAsync(reservationId.Value, userId, ct);
                }
                else if (offerId.HasValue)
                {
                    var reservation = await _reservationService.ReserveOfferAsync(offerId.Value, userId, ct);

                    await _reservationService.PurchaseReservationAsync(reservation.Id, userId, ct);
                }

                TempData["SuccessMessage"] = "გილოცავთ! კუპონი წარმატებით შეიძინეთ.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "შეცდომა ყიდვისას: " + ex.Message;
            }

            return RedirectToAction("MyCoupons", "Customer");
        }
    }
}
