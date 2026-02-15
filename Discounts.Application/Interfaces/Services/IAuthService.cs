// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Auth;

namespace Discounts.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequest request);

        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}
