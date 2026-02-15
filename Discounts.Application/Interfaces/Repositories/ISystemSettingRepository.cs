// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.Repositories
{
    public interface ISystemSettingRepository : IBaseRepository<SystemSetting>
    {
        Task<SystemSetting?> GetCurrentSettingsAsync(CancellationToken ct);
    }
}
