using BankAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Data
{
    public class ApiContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }

        public DbSet<DebitCard> DebitCards { get; set; }

        public DbSet<Document> Documents { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<PartialPassword> PartialPasswords { get; set; }

        public DbSet<SessionToken> SessionTokens { get; set; }

        public DbSet<Transfer> Transfers { get; set; }

        public ApiContext(DbContextOptions<ApiContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(x => x.PartialPasswords)
                .WithOne()
                .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<User>()
                .HasMany(x => x.SessionTokens)
                .WithOne()
                .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<Account>()
                .HasMany(x => x.DebitCards)
                .WithOne()
                .HasForeignKey(x => x.AccountId);

            modelBuilder.Entity<Account>()
                .HasMany(x => x.Transfers)
                .WithOne()
                .HasForeignKey(x => x.IssuerId);

            modelBuilder.Entity<Account>()
                .HasMany(x => x.Transfers)
                .WithOne()
                .HasForeignKey(x => x.ReceiverId);

            modelBuilder.Entity<User>()
                .HasOne(x => x.Account)
                .WithOne()
                .HasForeignKey<Account>(x => x.UserId);
            
            modelBuilder.Entity<Account>()
                .HasOne(x => x.Document)
                .WithOne()
                .HasForeignKey<Document>(x => x.AccountId);
        }
    }
}