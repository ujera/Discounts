// Copyright (C) TBC Bank. All Rights Reserved.

using System.Diagnostics;
using Discounts.Application.Interfaces.Services;
using Discounts.MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOfferService _offerService;

        public HomeController(ILogger<HomeController> logger, IOfferService offerService)
        {
            _logger = logger;
            _offerService = offerService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var activeOffers = await _offerService.GetActiveOffersAsync(ct);
            return View(activeOffers);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
