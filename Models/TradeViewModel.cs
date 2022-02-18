namespace MyCryptoMarket_MVC.Models
{
    public class TradeViewModel
    {
        public string Symbol { get; set; }

        public decimal LastPrice { get; set; }

        public decimal PriceChangePercent { get; set; }

        public decimal PriceChangePercent100 { get; set; }

        public decimal Volume { get; set; }
    }
}
