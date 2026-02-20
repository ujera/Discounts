// Copyright (C) TBC Bank. All Rights Reserved.

using System.Diagnostics;
using Discounts.Application.DTOs.Offer;
using Discounts.Application.Interfaces.Services;
using Discounts.Application.Services;
using Discounts.MVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Discounts.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOfferService _offerService;
        private readonly ICategoryService _categoryService;

        public HomeController(ILogger<HomeController> logger, IOfferService offerService, ICategoryService categoryService)
        {
            _logger = logger;
            _offerService = offerService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index([FromQuery] OfferFilterDto filter, CancellationToken ct)
        {
            var pagedOffers = await _offerService.GetAllActiveAsync(filter, ct);

            var categories = await _categoryService.GetAllAsync(ct);
            ViewBag.Categories = new SelectList(categories, "Id", "Name", filter.CategoryId);

            ViewBag.CurrentFilter = filter;

            return View(pagedOffers);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("Home/Error/{statusCode?}")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode)
        {
            if (statusCode.HasValue && statusCode.Value == 404)
            {
                return View("NotFound404");
            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
