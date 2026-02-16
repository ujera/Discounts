// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Auth;
using Discounts.Application.Exceptions.ResponceFormat;
using Discounts.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.API.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new Merchant or Customer.
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ApiResponse<string>>> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {
            await _authService.RegisterAsync(request);
            return OkResponse("Registration successful. You can now login.");
        }

        /// <summary>
        /// Logs in and returns a JWT Token.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var result = await _authService.LoginAsync(request);
            return OkResponse(result, "Login successful.");
        }
    }
}
