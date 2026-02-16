// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.User;
using Discounts.Application.Exceptions.ResponceFormat;
using Discounts.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : BaseApiController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Gets all users. Optional: filter by role (Merchant/Customer).
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserDto>>), 200)]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetAll([FromQuery] string? role)
        {
            var result = await _userService.GetAllUsersAsync(role);
            return OkResponse(result);
        }

        /// <summary>
        /// Gets a user by ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetById(string id)
        {
            var result = await _userService.GetByIdAsync(id);
            return OkResponse(result);
        }

        /// <summary>
        /// Edits user details (First/Last Name).
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ApiResponse<string>>> Update(string id, [FromBody] UpdateUserDto dto)
        {
            await _userService.UpdateUserAsync(id, dto);
            return OkResponse("User updated successfully.");
        }

        /// <summary>
        /// Blocks or Unblocks a user.
        /// </summary>
        [HttpPost("{id}/toggle-block")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ApiResponse<string>>> ToggleBlock(string id)
        {
            await _userService.BlockUserAsync(id);
            return OkResponse("User status changed successfully.");
        }

        /// <summary>
        /// Permanently deletes a user.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ApiResponse<string>>> Delete(string id)
        {
            await _userService.DeleteUserAsync(id);
            return OkResponse("User deleted successfully.");
        }
    }
}
