// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Category;
using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Application.Interfaces.Services;
using Discounts.Domain.Entities;
using Mapster;

namespace Discounts.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken ct = default)
        {
            var categories = await _unitOfWork.Categories.GetAllAsync(ct).ConfigureAwait(false);
            return categories.Adapt<IEnumerable<CategoryDto>>();
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken ct = default)
        {
            var existing = await _unitOfWork.Categories.FindAsync(c => c.Name == dto.Name, ct).ConfigureAwait(false);
            if (existing.Any())
                throw new BadRequestException($"Category '{dto.Name}' already exists.");

            var category = dto.Adapt<Category>();

            await _unitOfWork.Categories.AddAsync(category, ct).ConfigureAwait(false);
            await _unitOfWork.SaveAsync(ct).ConfigureAwait(false);

            return category.Adapt<CategoryDto>();
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id, ct).ConfigureAwait(false);
            if (category == null)
                throw new NotFoundException("Category", id);

            var offers = await _unitOfWork.Offers.FindAsync(o => o.CategoryId == id, ct).ConfigureAwait(false);
            if (offers.Any())
                throw new BadRequestException("Cannot delete category with existing offers.");

            _unitOfWork.Categories.Remove(category);
            await _unitOfWork.SaveAsync(ct).ConfigureAwait(false);
        }
    }
}
