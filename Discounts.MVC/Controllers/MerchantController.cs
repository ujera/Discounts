// Copyright (C) TBC Bank. All Rights Reserved.

using System.Security.Claims;
using Discounts.Application.DTOs.Offer;
using Discounts.Application.Interfaces.Services;
using Discounts.MVC.Models.Merchant;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Discounts.MVC.Controllers
{
    [Authorize(Roles = "Merchant")]
    public class MerchantController : Controller
    {
        private readonly IOfferService _offerService;
        private readonly ICategoryService _categoryService;

        public MerchantController(IOfferService offerService, ICategoryService categoryService)
        {
            _offerService = offerService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> CreateOffer(CancellationToken ct)
        {
            var viewModel = new CreateOfferViewModel
            {
                Categories = await GetCategorySelectList(ct)
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOffer(CreateOfferViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategorySelectList(ct);
                return View(model);
            }

            var dto = model.Adapt<CreateOfferDto>();

            var merchantId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(merchantId))
            {
                return RedirectToAction("Login", "Auth");
            }

            await _offerService.CreateAsync(dto, merchantId, ct);


            TempData["SuccessMessage"] = "შეთავაზება წარმატებით შეიქმნა და ელოდება ადმინისტრატორის დასტურს!";
            return RedirectToAction(nameof(CreateOffer)); // დროებით ისევ აქ დავაბრუნოთ
        }

        private async Task<IEnumerable<SelectListItem>> GetCategorySelectList(CancellationToken ct)
        {
            var categories = await _categoryService.GetAllAsync(ct);
            return categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });
        }
    }
}
