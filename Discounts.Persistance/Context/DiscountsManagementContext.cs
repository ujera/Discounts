using Discounts.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Persistance.Context
{
    public class DiscountsManagementContext : IdentityDbContext<ApplicationUser>
    {
        public DiscountsManagementContext(DbContextOptions<DiscountsManagementContext> options) : base(options) { }

        // DbSets
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 


            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DiscountsManagementContext).Assembly);
        }
    }
}
