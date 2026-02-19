// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Offer;
using Discounts.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IOfferService _offerService;

        public AdminController(IOfferService offerService)
        {
            _offerService = offerService;
        }

        [HttpGet]
        public async Task<IActionResult> PendingOffers(CancellationToken ct)
        {
            var pendingOffers = await _offerService.GetPendingOffersAsync(ct);

            return View(pendingOffers);
        }

        [HttpPost]
        public async Task<IActionResult> HandleOfferAction(int offerId, bool isApproved, CancellationToken ct)
        {
            var dto = new AdminActionDto
            {
                OfferId = offerId,
                IsApproved = isApproved
            };
            await _offerService.ApproveOfferAsync(dto, ct);

            return RedirectToAction(nameof(PendingOffers));
        }
    }
}
