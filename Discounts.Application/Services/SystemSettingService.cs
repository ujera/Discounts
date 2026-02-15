// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Admin;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Application.Interfaces.Services;
using Discounts.Domain.Entities;
using Mapster;

namespace Discounts.Application.Services
{
    public class SystemSettingService : ISystemSettingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SystemSettingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SystemSettingsDto> GetSettingsAsync(CancellationToken ct = default)
        {
            var settings = await _unitOfWork.Settings.GetCurrentSettingsAsync(ct).ConfigureAwait(false);
            if (settings == null)
            {
                return new SystemSettingsDto { ReservationDurationInMinutes = 30, MerchantEditWindowInHours = 24 };
            }
            return settings.Adapt<SystemSettingsDto>();
        }

        public async Task UpdateSettingsAsync(SystemSettingsDto dto, CancellationToken ct = default)
        {
            var settings = await _unitOfWork.Settings.GetCurrentSettingsAsync(ct).ConfigureAwait(false);

            if (settings == null)
            {
                settings = dto.Adapt<SystemSetting>();
                await _unitOfWork.Settings.AddAsync(settings, ct).ConfigureAwait(false);
            }
            else
            {
                dto.Adapt(settings);
                _unitOfWork.Settings.Update(settings);
            }

            await _unitOfWork.SaveAsync(ct).ConfigureAwait(false);
        }
    }
}
