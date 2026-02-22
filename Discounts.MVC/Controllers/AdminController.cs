// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Admin;
using Discounts.Application.DTOs.Category;
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
        private readonly ICategoryService _categoryService;
        private readonly ISystemSettingService _settingsService;
        private readonly IUserService _userService;

        public AdminController(IOfferService offerService, ICategoryService categoryService, ISystemSettingService systemSettingService, IUserService userService)
        {
            _offerService = offerService;
            _categoryService = categoryService;
            _settingsService = systemSettingService;
            _userService = userService;
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
        [HttpPost]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                TempData["Error"] = "გთხოვთ მიუთითოთ უარყოფის მიზეზი";
                return RedirectToAction("PendingOffers");
            }

            await _offerService.RejectOfferAsync(id, reason);
            return RedirectToAction("PendingOffers");
        }

        // კატეგორიების მართვა
        [HttpGet]
        public async Task<IActionResult> Categories(CancellationToken ct)
        {
            var categories = await _categoryService.GetAllAsync(ct);
            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto dto, CancellationToken ct)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.CreateAsync(dto, ct);
                TempData["SuccessMessage"] = "კატეგორია წარმატებით დაემატა.";
            }
            return RedirectToAction(nameof(Categories));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id, CancellationToken ct)
        {
            try
            {
                await _categoryService.DeleteAsync(id, ct);
                TempData["SuccessMessage"] = "კატეგორია წაიშალა.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "შეცდომა წაშლისას: " + ex.Message;
            }
            return RedirectToAction(nameof(Categories));
        }

        //(Settings)
        [HttpGet]
        public async Task<IActionResult> Settings(CancellationToken ct)
        {
            var settings = await _settingsService.GetSettingsAsync(ct);
            return View(settings);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSettings(SystemSettingsDto dto, CancellationToken ct)
        {
            if (ModelState.IsValid)
            {
                await _settingsService.UpdateSettingsAsync(dto, ct);
                TempData["SuccessMessage"] = "პარამეტრები განახლდა.";
            }
            return RedirectToAction(nameof(Settings));
        }

        //Users
        [HttpGet]
        public async Task<IActionResult> Users(string? role)
        {
            var users = await _userService.GetAllUsersAsync(role);
            ViewBag.SelectedRole = role;
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleBlockUser(string id)
        {
            try
            {
                await _userService.BlockUserAsync(id);
                TempData["SuccessMessage"] = "მომხმარებლის სტატუსი შეიცვალა.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                TempData["SuccessMessage"] = "მომხმარებელი წაიშალა.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Users));
        }
    }
}
