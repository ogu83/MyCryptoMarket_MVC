using System;
using System.Linq;
using MyCryptoMarket_MVC.Models;
using System.Threading.Tasks;

namespace MyCryptoMarket_MVC.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(CryptoMarketContext context)
        {
            bool anyChanges = false;

            if (!context.Tickers.Any())
            {
                var binanceClient = new Binance.Net.BinanceClient();
                var binanceTickersResponse = await binanceClient.Spot.Market.GetTickersAsync();
                if (binanceTickersResponse.Success)
                {
                    var tickers = binanceTickersResponse.Data.Select(x => new Ticker
                    {
                        AskPrice = x.AskPrice,
                        BidPrice = x.BidPrice,
                        CloseTime = x.CloseTime,
                        LastPrice = x.LastPrice,
                        LastQty = x.LastQuantity,
                        LowPrice = x.LowPrice,
                        HighPrice = x.HighPrice,
                        OpenPrice = x.OpenPrice,
                        OpenTime = x.OpenTime,
                        PrevClosePrice = x.PrevDayClosePrice,
                        PriceChange = x.PriceChange,
                        PriceChangePercent = x.PriceChangePercent,
                        QuoteVolume = x.QuoteVolume,
                        Symbol = x.Symbol,
                        Volume = x.BaseVolume,
                        WeightedAvgPrice  =x.WeightedAveragePrice 
                    });

                    await context.Tickers.AddRangeAsync(tickers);
                    anyChanges = true;
                }
                else 
                {
                    throw new Exception(binanceTickersResponse.Error.Message);
                }

                if (anyChanges)
                    context.SaveChanges();
            }
        }
    }
}