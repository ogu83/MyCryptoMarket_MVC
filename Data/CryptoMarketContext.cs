using Microsoft.EntityFrameworkCore;
using MyCryptoMarket_MVC.Models;

namespace MyCryptoMarket_MVC.Data
{
    public class CryptoMarketContext : DbContext
    {
        public CryptoMarketContext (DbContextOptions<CryptoMarketContext> options)
            : base(options)
        {

        }

        public DbSet<Kline> Klines { get; set; }
        
        public DbSet<Ticker> Tickers { get; set;}

        public DbSet<Symbol> Symbols { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Balance> Balances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Kline>().ToTable("Kline")
                                        .HasIndex(x => new { x.Symbol, x.Interval, x.OpenTime, x.CloseTime });
                                        
            modelBuilder.Entity<Ticker>().ToTable("Ticker");


            modelBuilder.Entity<Symbol>().ToTable("Symbol")
                                         .HasIndex(x => new { x.Name });

            modelBuilder.Entity<User>().ToTable("User")
                                       .HasIndex(x => new { x.Name });

            modelBuilder.Entity<Balance>().ToTable("Balance")
                                          .HasIndex(x => new { x.AssetName, x.User_Id });
        }
    }
}