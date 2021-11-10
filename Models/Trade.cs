using System;
using System.ComponentModel.DataAnnotations;

namespace MyCryptoMarket_MVC.Models
{
    public enum TradeType { BUY, SELL }

    public class Trade
    {
        [Key]
        public int Id { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public DateTime? ExecutionDate { get ; set; }

        public TradeType TradeType { get; set; }

        public decimal Amount { get; set; }

        public decimal Price { get; set; }

        public decimal Total { get { return Amount * Price; } }
    }
}