using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyCryptoMarket_MVC.Data;
using MyCryptoMarket_MVC.Models;

namespace MyCryptoMarket_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CryptoMarketContext _context;
        private readonly MvcOptions _mvcOptions;

        public HomeController(ILogger<HomeController> logger,
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

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)           
            {
                var query = _context.Tickers.ToList();
                var myQuery = query.Where(x => x.WeightedAvgPrice > 0);
                var tickers = Newtonsoft.Json.JsonConvert.SerializeObject((myQuery.ToList()));
                ViewData["Tickers"] = tickers;
            }
            
            return View();
        }

        [HttpPost]
        public IActionResult OnGet24hTickers(DxGridRequest args)
        {
            var query = _context.Tickers.AsQueryable();
            var totalCount = query.Count();

            if (args.sort != null)
            {
                //TODO: Sort Here
            }

            if (args.take > 0)
            {
                query = query.Take(args.take);
            }

            if (args.skip > 0)
            {
                query = query.Skip(args.skip);
            }

            var tickers = query.ToList();
            var model = new DxGridResponse<Models.Ticker>(tickers, totalCount);
            var retval = new JsonResult(model);
            return retval;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
