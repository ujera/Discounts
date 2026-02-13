
using Discounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Discounts.Persistance.Configurations
{
    public class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
    {
        public void Configure(EntityTypeBuilder<SystemSetting> builder)
        {
            builder.ToTable("SystemSettings");


            builder.Property(x => x.ReservationDurationInMinutes)
                .IsRequired()
                .HasDefaultValue(30);

            builder.Property(x => x.MerchantEditWindowInHours)
                .IsRequired()
                .HasDefaultValue(24);

        }
    }
}
