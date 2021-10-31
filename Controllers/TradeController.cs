using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyCryptoMarket_MVC.Data;
using MyCryptoMarket_MVC.Models;
using Microsoft.AspNetCore.Authorization;

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

        public IActionResult Index(string Symbol)
        {
            var viewModel = new TradeViewModel();
            viewModel.Symbol = Symbol;
            return View(viewModel);
        }
    }
}
