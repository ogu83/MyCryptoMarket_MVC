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
        public static async Task InitializeUser(CryptoMarketContext context, string userName) 
        {
            var user = context.Users.FirstOrDefault(x=>x.Name == userName);
            await context.Users.AddAsync(new User { Name = userName });
            await context.SaveChangesAsync();
        }

        public static async Task InitializeBalances(CryptoMarketContext context, string userName)
        {
            var user = context.Users.FirstOrDefault(x=>x.Name == userName);
            if (user == null) 
            {
                return;
            }

            if (context.Balances.Any(x=>x.User_Id == user.Id))
            {
                return;
            }

            var baseAssets = context.Symbols.Select(x => x.BaseAsset);
            var quotaAssets = context.Symbols.Select(x => x.QuoteAsset);
            var assets = baseAssets.Union(quotaAssets);
            assets = assets.Distinct();            
            var balances = assets.Select(x => new Balance 
            {
                User_Id = user.Id,
                AssetName = x,
                AmountInUse = 0,
                TotalAmount = x == "ETH" ? 1000 : x == "BTC" ? 100 : x == "USDT" ? 10000 : 0
            });

            await context.Balances.AddRangeAsync(balances);
            await context.SaveChangesAsync();
        }
        
        public static async Task InitializeExchangeInfo(CryptoMarketContext context)
        {
            if (context.Symbols.Any())
            {
                return;
            }

            var binanceClient = new BinanceClient();
            var exchangeInfoResponse = await binanceClient.Spot.System.GetExchangeInfoAsync();
            if (exchangeInfoResponse.Success) 
            {
                var symbols = exchangeInfoResponse.Data.Symbols.Select(x => new Symbol 
                {
                    Name = x.Name,
                    Status = x.Status,
                    BaseAsset = x.BaseAsset,
                    BaseAssetPrecision = x.BaseAssetPrecision,
                    QuoteAsset = x.QuoteAsset,
                    QuotePrecision = x.QuoteAssetPrecision,
                    BaseCommissionPrecision = x.BaseCommissionPrecision,
                    QuoteCommissionPrecision = x.QuoteCommissionPrecision
                });

                await context.Symbols.AddRangeAsync(symbols);
                await context.SaveChangesAsync();
            }
        }

        public static async Task InitializeKline(CryptoMarketContext context, string Symbol, KlineInterval Interval, DateTime StartDate, DateTime EndDate) 
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
                    foreach (var kline in klines)
                    {
                        var cKline = context.Klines.FirstOrDefault(x => x.Symbol == kline.Symbol && x.OpenTime == kline.OpenTime && x.CloseTime == kline.CloseTime && x.Interval == kline.Interval);
                        if (cKline == null) 
                        {
                            await context.Klines.AddAsync(kline);
                        }
                        else 
                        {
                            cKline.Low = kline.Low;
                            cKline.High = kline.High;
                            cKline.Open = kline.Open;
                            cKline.Close = kline.Close;
                            cKline.Volume = kline.Volume;                            
                            context.Klines.Update(cKline);
                        }
                    }     

                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _ = ex;
                }
            }
        }

        public static async Task Initialize(CryptoMarketContext context)
        {
            var anyChanges = false;

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
                {
                    await context.SaveChangesAsync();
                }                    
            }
        }
    }
}