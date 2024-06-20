using Kallum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kallum.Data
{
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }
        //will add dbset for models later
        public DbSet<UserAccount> UserAccountsData { get; set; }
        public DbSet<BankAccount> BankAccountsData { get; set; }
        public DbSet<UserBankAccountInformation> UserBankAccountInformationData { get; set; }
        public DbSet<TransactionHistory> TransactionHistoriesData { get; set; }
        public DbSet<BalanceDetails> BalanceDetailsData { get; set; }
        public DbSet<KallumLock> KallumLockData { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //will add foreign keys later
            builder.Entity<UserAccount>()
                       .HasOne(b => b.BankAccount);
            builder.Entity<UserAccount>()
                       .HasOne(b => b.BalanceDetails);



            //
            List<IdentityRole> roles = new List<IdentityRole> {
                new  IdentityRole{
                Name="Admin",
                NormalizedName="ADMIN"
                },
            new IdentityRole
            {
                Name="User",
                NormalizedName="USER"
            }
            };
            builder.Entity<IdentityRole>().HasData(roles);


        }
    }
}
