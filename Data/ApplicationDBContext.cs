using Kallum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Kallum.Data
{
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<UserAccount> UserAccountsData { get; set; }
        public DbSet<BankAccount> BankAccountsData { get; set; }
        public DbSet<UserBankAccountInformation> UserBankAccountInformationData { get; set; }
        public DbSet<TransactionHistory> TransactionHistoriesData { get; set; }
        public DbSet<BalanceDetails> BalanceDetailsData { get; set; }
        public DbSet<KallumLock> KallumLockData { get; set; }
        public DbSet<FinanceCircle> FinanceCircleData { get; set; }
        public DbSet<Circle> CircleData { get; set; }
        public DbSet<GeneralNotification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure GUIDs to use UUID in PostgreSQL
            builder.Entity<FinanceCircle>(entity =>
            {
                entity.Property(e => e.CircleId).HasColumnType("uuid");
            });

            // Composite key configuration
            builder.Entity<UserBankAccountInformation>(entity =>
            {
                entity.HasKey(e => e.AppUserId);
            });

            builder.Entity<UserBankAccountInformation>()
                .HasOne(u => u.AppUser)
                .WithMany(t => t.FullInformation)
                .HasForeignKey(k => k.AppUserId);

            // Seed roles
            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Name = "User", NormalizedName = "USER" }
            };
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
