using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyCryptoMarket_MVC.Data;
using MyCryptoMarket_MVC.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Binance.Net.Enums;
using MyCryptoMarket_MVC.Helper;

namespace MyCryptoMarket_MVC.Controllers
{
    [Authorize]
    public class TradeController: Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CryptoMarketContext _context;
        private readonly MvcOptions _mvcOptions;

        public TradeController(ILogger<HomeController> logger,
            CryptoMarketContext context, 
            IOptions<MvcOptions> mvcOptions)
        {
            _logger = logger;
            _context = context;
            if (mvcOptions != null)
            {
                _mvcOptions = mvcOptions.Value;
            }
        }

        public async Task<IActionResult> Index(string Symbol)
        {
            var viewModel = new TradeViewModel();
            viewModel.Symbol = Symbol;
            await DbInitializer.InitializeKline(_context, Symbol, Binance.Net.Enums.KlineInterval.OneDay, DateTime.Today.AddMonths(-1), DateTime.Today);

            var ticker = _context.Tickers.FirstOrDefault(x => x.Symbol == Symbol);
            if (ticker != null) 
            {
                viewModel.LastPrice = ticker.LastPrice;
                viewModel.PriceChangePercent = ticker.PriceChangePercent;
                viewModel.PriceChangePercent100 = ticker.PriceChangePercent100;
                viewModel.Volume = ticker.Volume;
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> GetKlines(KlineRequest request)
        {
            await DbInitializer.InitializeKline(_context, request.Symbol, request.Interval, request.StartDate, request.EndDate);

            var query = _context.Klines.Where(k => k.Interval == request.Interval 
                                                && k.Symbol == request.Symbol 
                                                && k.OpenTime >= request.StartDate
                                                && k.CloseTime <= request.EndDate);

            var retval = query.ToList();

            return Json(retval);
        }

        [HttpPost]
        public ActionResult GetIntervals()
        {
            var retval = new List<IdValueSelectable>();
            foreach (KlineInterval interval in (KlineInterval[]) Enum.GetValues(typeof(KlineInterval)))
            {
                retval.Add(new IdValueSelectable 
                { 
                    Id = (int)interval, 
                    Value = Enum.GetName(typeof(KlineInterval), interval), 
                    IsSelected = interval == KlineInterval.OneDay 
                });                
            }
            return Json(retval);
        }
    }
}