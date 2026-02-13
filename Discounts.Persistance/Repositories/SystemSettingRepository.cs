// Copyright (C) TBC Bank. All Rights Reserved.


using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Entities;
using Discounts.Persistance.Context;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Persistance.Repositories
{
    public class SystemSettingRepository : BaseRepository<SystemSetting>, ISystemSettingRepository
    {
        public SystemSettingRepository(DiscountsManagementContext context) : base(context) { }

        public async Task<SystemSetting?> GetCurrentSettingsAsync()
        {
            return await _context.SystemSettings.FirstOrDefaultAsync().ConfigureAwait(false);
        }
    }
}
