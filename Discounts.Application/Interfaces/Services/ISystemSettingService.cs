// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Admin;

namespace Discounts.Application.Interfaces.Services
{
    public interface ISystemSettingService
    {
        Task<SystemSettingsDto> GetSettingsAsync(CancellationToken ct = default);
        Task UpdateSettingsAsync(SystemSettingsDto dto, CancellationToken ct = default);
    }
}
