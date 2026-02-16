// Copyright (C) TBC Bank. All Rights Reserved.

using System.Security.Claims;
using Discounts.Application.DTOs.Merchant;
using Discounts.Application.Exceptions.ResponceFormat;
using Discounts.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.API.Controllers
{
    [Authorize(Roles = "Merchant")]
    public class MerchantController : BaseApiController
    {
        private readonly IMerchantService _merchantService;

        public MerchantController(IMerchantService merchantService)
        {
            _merchantService = merchantService;
        }

        /// <summary>
        /// Gets the detailed sales history
        /// </summary>
        [HttpGet("sales-history")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<MerchantSalesDto>>), 200)]
        public async Task<ActionResult<ApiResponse<IEnumerable<MerchantSalesDto>>>> GetSalesHistory(CancellationToken ct)
        {
            var merchantId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _merchantService.GetSalesHistoryAsync(merchantId!, ct);
            return OkResponse(result);
        }

        /// <summary>
        /// Gets dashboard statistics
        /// </summary>
        [HttpGet("stats")]
        [ProducesResponseType(typeof(ApiResponse<MerchantStatsDto>), 200)]
        public async Task<ActionResult<ApiResponse<MerchantStatsDto>>> GetDashboardStats(CancellationToken ct)
        {
            var merchantId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _merchantService.GetDashboardStatsAsync(merchantId!, ct);
            return OkResponse(result);
        }
    }
}
