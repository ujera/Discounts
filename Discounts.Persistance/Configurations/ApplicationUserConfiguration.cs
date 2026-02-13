using Discounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Discounts.Persistence.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("Users");
            builder.Property(x => x.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.LastName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
        }
    }
}
