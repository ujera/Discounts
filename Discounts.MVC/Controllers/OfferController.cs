// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.MVC.Controllers
{
    public class OfferController : Controller
    {
        private readonly IOfferService _offerService;

        public OfferController(IOfferService offerService)
        {
            _offerService = offerService;
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            try
            {
                var offer = await _offerService.GetByIdAsync(id, ct);

                if (offer == null)
                    return NotFound();

                return View(offer);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}
