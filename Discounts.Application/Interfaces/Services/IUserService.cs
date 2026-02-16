// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.User;

namespace Discounts.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync(string? role = null);
        Task<UserDto> GetByIdAsync(string id);
        Task UpdateUserAsync(string id, UpdateUserDto dto);
        Task BlockUserAsync(string id);
        Task DeleteUserAsync(string id);
    }
}
