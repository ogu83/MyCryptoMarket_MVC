using System;
using System.ComponentModel.DataAnnotations;

namespace MyCryptoMarket_MVC.Models
{
    public class Ticker
    {
        //{
        //   "symbol": "BNBBTC",
        //   "priceChange": "-94.99999800",
        //   "priceChangePercent": "-95.960",
        //   "weightedAvgPrice": "0.29628482",
        //   "prevClosePrice": "0.10002000",
        //   "lastPrice": "4.00000200",
        //   "lastQty": "200.00000000",
        //   "bidPrice": "4.00000000",
        //   "askPrice": "4.00000200",
        //   "openPrice": "99.00000000",
        //   "highPrice": "100.00000000",
        //   "lowPrice": "0.10000000",
        //   "volume": "8913.30000000",
        //   "quoteVolume": "15.30000000",
        //   "openTime": 1499783499040,
        //   "closeTime": 1499869899040,
        //   "firstId": 28385,   // First tradeId
        //   "lastId": 28460,    // Last tradeId
        //   "count": 76         // Trade count
        //}

        [Key]
        public string Symbol { get; set; }

        public decimal PriceChange { get; set; }

        public decimal PriceChangePercent { get; set; }

        public decimal WeightedAvgPrice { get; set; }

        public decimal PrevClosePrice { get; set; }

        public decimal LastPrice { get; set; }

        public decimal LastQty { get; set; }

        public decimal BidPrice { get; set; }

        public decimal AskPrice { get; set; }

        public  decimal OpenPrice { get; set; }

        public decimal LowPrice { get; set; }

        public decimal HighPrice { get; set; }

        public decimal Volume { get; set; }

        public decimal QuoteVolume { get; set; }

        public DateTime OpenTime { get; set; }

        public DateTime CloseTime { get; set; }
    }
}
