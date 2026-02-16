// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.User;
using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Services;
using Discounts.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync(string? role = null)
        {
            var query = _userManager.Users.AsQueryable();

            var users = await query.ToListAsync().ConfigureAwait(false);
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
                if (string.IsNullOrEmpty(role) || roles.Contains(role))
                {
                    var dto = user.Adapt<UserDto>();
                    dto.Role = roles.FirstOrDefault() ?? "Unknown";
                    userDtos.Add(dto);
                }
            }

            return userDtos;
        }

        public async Task<UserDto> GetByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id).ConfigureAwait(false);
            if (user == null) throw new NotFoundException("User", id);

            var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            var dto = user.Adapt<UserDto>();
            dto.Role = roles.FirstOrDefault() ?? "Unknown";

            return dto;
        }

        public async Task UpdateUserAsync(string id, UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(id).ConfigureAwait(false) ?? throw new NotFoundException("User", id);
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;

            var result = await _userManager.UpdateAsync(user).ConfigureAwait(false);
            if (!result.Succeeded)
                throw new BadRequestException("Failed to update user.");
        }

        public async Task BlockUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id).ConfigureAwait(false) ?? throw new NotFoundException("User", id);

            var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            if (roles.Contains("Admin"))
                throw new BadRequestException("You cannot block an Administrator.");

            user.IsActive = !user.IsActive;

            await _userManager.UpdateAsync(user).ConfigureAwait(false);
        }

        public async Task DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id).ConfigureAwait(false) ?? throw new NotFoundException("User", id);
            
            await _userManager.DeleteAsync(user).ConfigureAwait(false);
        }
    }
}
