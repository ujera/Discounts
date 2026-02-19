// Copyright (C) TBC Bank. All Rights Reserved.

using System.Security.Claims;
using Discounts.Application.DTOs.Offer;
using Discounts.Application.Exceptions.ResponceFormat;
using Discounts.Application.Interfaces.Services;
using Discounts.Application.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.API.Controllers
{
    public class OffersController : BaseApiController
    {
        private readonly IOfferService _offerService;

        public OffersController(IOfferService offerService)
        {
            _offerService = offerService;
        }

        /// <summary>
        /// Gets all active offers with pagination and filtering.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<OfferDto>>), 200)]
        public async Task<ActionResult<ApiResponse<PagedResult<OfferDto>>>> GetAll([FromQuery] OfferFilterDto filter, CancellationToken ct)
        {
            var result = await _offerService.GetAllActiveAsync(filter, ct);
            return OkResponse(result);
        }

        /// <summary>
        /// Gets an offer details.
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<OfferDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<OfferDto>>> GetById(int id, CancellationToken ct)
        {
            var result = await _offerService.GetByIdAsync(id, ct);
            return OkResponse(result);
        }

        /// <summary>
        /// Creates a new offer.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Merchant")]
        [ProducesResponseType(typeof(ApiResponse<OfferDto>), 201)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ApiResponse<OfferDto>>> Create([FromBody] CreateOfferDto dto, CancellationToken ct)
        {
            var merchantId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _offerService.CreateAsync(dto, merchantId!, ct);
            return CreatedResponse(result, "Offer created successfully. Waiting for Admin approval.");
        }

        /// <summary>
        /// Updates an existing offer.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Merchant")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ApiResponse<string>>> Update(int id, [FromBody] UpdateOfferDto dto, CancellationToken ct)
        {
            var merchantId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _offerService.UpdateAsync(id, dto, merchantId!, ct);
            return OkResponse("Offer updated successfully.");
        }

        /// <summary>
        ///View my own offers.
        /// </summary>
        [HttpGet("my-offers")]
        [Authorize(Roles = "Merchant")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OfferDto>>), 200)]
        public async Task<ActionResult<ApiResponse<IEnumerable<OfferDto>>>> GetMyOffers(CancellationToken ct)
        {
            var merchantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _offerService.GetMyOffersAsync(merchantId!,ct);
            return OkResponse(result);
        }
    }
}
