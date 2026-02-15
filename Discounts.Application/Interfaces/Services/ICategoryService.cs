// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Category;

namespace Discounts.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken ct = default);
        Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
