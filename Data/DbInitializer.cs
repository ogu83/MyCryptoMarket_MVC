using System;
using System.Linq;
using MyCryptoMarket_MVC.Models;
using System.Threading.Tasks;
using Binance.Net.Enums;
using Binance.Net;

namespace MyCryptoMarket_MVC.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeKline(
            CryptoMarketContext context, 
            string Symbol, 
            KlineInterval Interval, 
            DateTime StartDate, DateTime EndDate) 
        {
            var exist = context.Klines.Any(x=>x.Symbol == Symbol && x.CloseTime == EndDate && x.Interval == Interval) 
                        &&
                        context.Klines.Any(x => x.Symbol == Symbol && x.OpenTime == StartDate && x.Interval == Interval);

            if (exist) 
            {
                return;
            }

            var binanceClient = new BinanceClient();
            var klineResponse = await binanceClient.Spot.Market.GetKlinesAsync(Symbol, Interval, StartDate, EndDate);
            if (klineResponse.Success)
            {
                var klines = klineResponse.Data.Select(x => new Kline 
                {
                    Close = x.Close,
                    CloseTime = x.CloseTime,
                    High = x.High,
                    Interval = Interval,
                    Low = x.Low, 
                    Open = x.Open,
                    OpenTime = x.OpenTime,
                    Symbol = Symbol,
                    Volume = x.BaseVolume
                });

                try
                {
                    await context.Klines.AddRangeAsync(klines);
                    context.SaveChanges();
                }
                catch (System.Exception)
                {
                    
                }


            }
        }

        public static async Task Initialize(CryptoMarketContext context)
        {
            bool anyChanges = false;

            if (!context.Tickers.Any())
            {
                var binanceClient = new BinanceClient();
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