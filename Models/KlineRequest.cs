using System;
using Binance.Net.Enums;

namespace MyCryptoMarket_MVC.Models
{
    public class KlineRequest 
    {
        public string Symbol  { get; set; }

        public KlineInterval Interval  { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}