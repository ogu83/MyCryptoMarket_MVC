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
            modelBuilder.Entity<Kline>().ToTable("Kline");
            modelBuilder.Entity<Ticker>().ToTable("Ticker");
        }
    }
}
