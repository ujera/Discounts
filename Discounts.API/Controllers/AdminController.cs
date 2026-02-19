// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Admin;
using Discounts.Application.DTOs.Category;
using Discounts.Application.DTOs.Offer;
using Discounts.Application.Exceptions.ResponceFormat;
using Discounts.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseApiController
    {
        private readonly ISystemSettingService _settingsService;
        private readonly ICategoryService _categoryService;
        private readonly IOfferService _offerService;

        public AdminController(
            ISystemSettingService settingsService,
            ICategoryService categoryService,
            IOfferService offerService)
        {
            _settingsService = settingsService;
            _categoryService = categoryService;
            _offerService = offerService;
        }

        /// <summary>
        /// Gets the current system configuration.
        /// </summary>
        [HttpGet("settings")]
        [ProducesResponseType(typeof(ApiResponse<SystemSettingsDto>), 200)]
        public async Task<ActionResult<ApiResponse<SystemSettingsDto>>> GetSettings(CancellationToken ct)
        {
            var result = await _settingsService.GetSettingsAsync(ct);
            return OkResponse(result);
        }

        /// <summary>
        /// Updates system configuration.
        /// </summary>
        [HttpPut("settings")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ApiResponse<string>>> UpdateSettings([FromBody] SystemSettingsDto dto, CancellationToken ct)
        {
            await _settingsService.UpdateSettingsAsync(dto, ct);
            return OkResponse("System settings updated successfully.");
        }

        /// <summary>
        /// Creates a new category for offers.
        /// </summary>
        [HttpPost("categories")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> CreateCategory([FromBody] CreateCategoryDto dto, CancellationToken ct)
        {
            var result = await _categoryService.CreateAsync(dto, ct);
            return CreatedResponse(result, "Category created successfully.");
        }

        /// <summary>
        /// Deletes a category. Fails if offers exist in it.
        /// </summary>
        [HttpDelete("categories/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<string>>> DeleteCategory(int id, CancellationToken ct)
        {
            await _categoryService.DeleteAsync(id, ct);
            return OkResponse("Category deleted successfully.");
        }

        /// <summary>
        /// Approves or Rejects a merchant's offer.
        /// </summary>
        [HttpPost("offers/approve")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<string>>> ApproveOffer([FromBody] AdminActionDto dto, CancellationToken ct)
        {
            await _offerService.ApproveOfferAsync(dto, ct);

            var status = dto.IsApproved ? "Approved" : "Rejected";
            return OkResponse($"Offer has been {status}.");
        }

        /// <summary>
        /// Gets a list of all pending offers awaiting admin approval.
        /// </summary>
        [HttpGet("offers/pending")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OfferDto>>), 200)]
        public async Task<ActionResult<ApiResponse<IEnumerable<OfferDto>>>> GetPendingOffers(CancellationToken ct)
        {
            var result = await _offerService.GetPendingOffersAsync(ct);
            return OkResponse(result);
        }
    }
}
