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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Kline>().ToTable("Kline")
                                        .HasKey(x => new { x.Symbol, x.Interval, x.OpenTime, x.CloseTime });
                                        
            modelBuilder.Entity<Ticker>().ToTable("Ticker");
        }
    }
}