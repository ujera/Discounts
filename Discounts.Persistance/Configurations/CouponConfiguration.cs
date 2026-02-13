using Discounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Discounts.Persistance.Configurations
{
    public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.ToTable("Coupons");


            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(x => x.Code)
                .IsUnique();

            builder.Property(x => x.SoldAt)
                .HasColumnType("datetime2");

            builder.Property(x => x.IsUsed)
                .HasDefaultValue(false);

            // Relations
            builder.HasOne(x => x.Customer)
                .WithMany(x => x.Coupons)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Offer)
                .WithMany(x => x.Coupons)
                .HasForeignKey(x => x.OfferId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
