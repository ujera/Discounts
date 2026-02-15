// Copyright (C) TBC Bank. All Rights Reserved.

using System.Security.Claims;
using Discounts.Application.DTOs.Customer;
using Discounts.Application.Exceptions.ResponceFormat;
using Discounts.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.API.Controllers
{
    public class ReservationsController : BaseApiController
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        /// <summary>
        /// Locks an offer for 30 minutes.
        /// </summary>
        /// <param name="offerId">The ID of the offer to reserve.</param>
        [HttpPost("reserve/{offerId}")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(ApiResponse<ReservationResponseDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<ReservationResponseDto>>> Reserve(int offerId, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _reservationService.ReserveOfferAsync(offerId, userId!, ct).ConfigureAwait(true);

            return OkResponse(result, "Offer reserved successfully. You have 30 minutes to complete purchase.");
        }

        /// <summary>
        /// Completes the purchase for a specific reservation.
        /// </summary>
        /// <param name="reservationId">The ID of the active reservation.</param>
        [HttpPost("purchase/{reservationId}")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(ApiResponse<CouponDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ApiResponse<CouponDto>>> Purchase(int reservationId, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _reservationService.PurchaseReservationAsync(reservationId, userId!, ct);

            return OkResponse(result, "Purchase successful! Here is your coupon code.");
        }

        /// <summary>
        /// Gets all coupons purchased by the current user.
        /// </summary>
        [HttpGet("my-coupons")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CouponDto>>), 200)]
        public async Task<ActionResult<ApiResponse<IEnumerable<CouponDto>>>> GetMyCoupons(CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _reservationService.GetMyCouponsAsync(userId!, ct);

            return OkResponse(result);
        }
    }
}
