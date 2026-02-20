// Copyright (C) TBC Bank. All Rights Reserved.

using System.Security.Claims;
using Discounts.Application.DTOs.Offer;
using Discounts.Application.Interfaces.Services;
using Discounts.MVC.Models.Merchant;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Discounts.MVC.Controllers
{
    [Authorize(Roles = "Merchant")]
    public class MerchantController : Controller
    {
        private readonly IOfferService _offerService;
        private readonly ICategoryService _categoryService;
        private readonly IMerchantService _merchantService;
        private readonly ISystemSettingService _settingsService;

        public MerchantController(IOfferService offerService, ICategoryService categoryService, IMerchantService merchantService, ISystemSettingService settingsService)
        {
            _offerService = offerService;
            _categoryService = categoryService;
            _merchantService = merchantService;
            _settingsService = settingsService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var merchantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(merchantId)) return RedirectToAction("Login", "Auth");

            var stats = await _merchantService.GetDashboardStatsAsync(merchantId, ct);

            return View(stats);
        }
        [HttpGet]
        public async Task<IActionResult> MyOffers(CancellationToken ct)
        {
            var merchantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(merchantId)) return RedirectToAction("Login", "Auth");

            var myOffers = await _offerService.GetMyOffersAsync(merchantId, ct);

            return View(myOffers);
        }

        // 2. გაყიდვების ისტორია

        [HttpGet]
        public async Task<IActionResult> SalesHistory(CancellationToken ct)
        {
            var merchantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(merchantId)) return RedirectToAction("Login", "Auth");

            var sales = await _merchantService.GetSalesHistoryAsync(merchantId, ct);

            return View(sales);
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

        // რედაქტირების გვერდი
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var merchantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(merchantId)) return RedirectToAction("Login", "Auth");

            var offer = await _offerService.GetByIdAsync(id, ct);

            var settings = await _settingsService.GetSettingsAsync(ct);
            int editLimitHours = settings.MerchantEditWindowInHours;

            if (offer.CreatedAt.AddHours(editLimitHours) < DateTime.UtcNow)
            {
                TempData["ErrorMessage"] = $"რედაქტირების დრო ამოიწურა. ცვლილებების შეტანა შესაძლებელია მხოლოდ შექმნიდან {editLimitHours} საათის განმავლობაში.";
                return RedirectToAction(nameof(MyOffers));
            }

            var viewModel = offer.Adapt<EditOfferViewModel>();
            viewModel.Categories = await GetCategorySelectList(ct);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditOfferViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategorySelectList(ct);
                return View(model);
            }

            var merchantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(merchantId)) return RedirectToAction("Login", "Auth");

            try
            {
                var dto = model.Adapt<UpdateOfferDto>();

                await _offerService.UpdateAsync(model.Id, dto, merchantId, ct);

                TempData["SuccessMessage"] = "შეთავაზება წარმატებით განახლდა.";
                return RedirectToAction(nameof(MyOffers));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                model.Categories = await GetCategorySelectList(ct);
                return View(model);
            }
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
