using System;
using System.ComponentModel.DataAnnotations;

namespace MyCryptoMarket_MVC.Models
{
    public enum TradeType { BUY, SELL }

    public enum TradeStatus { ORDERED, PARTIAL_FILLED, FILLED, CANCELED }

    public class Trade
    {
        [Key]
        public int Id { get; set; }

        public int Symbol_Id { get; set; }        

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public DateTime? ExecutionDate { get ; set; }

        public TradeType TradeType { get; set; }

        public decimal Amount { get; set; }

        public decimal FilledAmount { get; set; }

        public decimal RemainAmount { get { return Amount - FilledAmount; } }                    

        public decimal Price { get; set; }

        public decimal Total { get { return Amount * Price; } }

        public decimal FilledTotal { get { return FilledAmount * Price; } }

        public decimal RemainTotal { get { return RemainAmount * Price; } }

        public TradeStatus Status { get; set; } = TradeStatus.ORDERED;
    }
}