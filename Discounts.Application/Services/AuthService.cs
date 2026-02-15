// Copyright (C) TBC Bank. All Rights Reserved.
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Discounts.Application.DTOs.Auth;
using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Services;
using Discounts.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Discounts.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
           IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email).ConfigureAwait(false);
            if (existingUser != null)
                throw new BadRequestException("User with this email already exists.");

            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, request.Password).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"Registration failed: {errors}");
            }

            if (!await _roleManager.RoleExistsAsync(request.Role).ConfigureAwait(false))
            {
                await _roleManager.CreateAsync(new IdentityRole(request.Role)).ConfigureAwait(false);
            }

            await _userManager.AddToRoleAsync(user, request.Role).ConfigureAwait(false);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email).ConfigureAwait(false);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password).ConfigureAwait(false))
                throw new BadRequestException("Invalid credentials.");

            if (!user.IsActive)
                throw new BadRequestException("Account is blocked.");

            var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

            var token = GenerateJwtToken(user, roles);

            return new AuthResponse
            {
                Token = token,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? "Customer",
                Expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes)
            };
        }

        private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new("FirstName", user.FirstName ?? ""),
                new("LastName", user.LastName ?? "")
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
