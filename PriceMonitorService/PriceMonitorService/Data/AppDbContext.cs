using Microsoft.EntityFrameworkCore;
using PriceMonitorService.Models;

namespace PriceMonitorService.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<PriceRecord> PriceRecords { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscription>()
                .HasIndex(s => s.ListingUrl)
                .IsUnique(false);

            modelBuilder.Entity<PriceRecord>()
                .HasIndex(pr => pr.SubscriptionId);
        }
    }
}
