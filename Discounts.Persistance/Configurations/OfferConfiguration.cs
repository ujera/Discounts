using Discounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Discounts.Persistance.Configurations
{
    public class OfferConfiguration : IEntityTypeConfiguration<Offer>
    {
        public void Configure(EntityTypeBuilder<Offer> builder)
        {
            builder.ToTable("Offers");

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(x => x.ImageUrl)
                .IsRequired(false);

            // for money 
            builder.Property(x => x.OriginalPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.DiscountPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.StartDate)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(x => x.EndDate)
                .IsRequired()
                .HasColumnType("datetime2");

          
            builder.Property(x => x.Status)
                .HasConversion<string>()
                .IsRequired();

            // Relations
            builder.HasOne(x => x.Merchant)
                .WithMany(x => x.Offers)
                .HasForeignKey(x => x.MerchantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Category)
                .WithMany(x => x.Offers)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
