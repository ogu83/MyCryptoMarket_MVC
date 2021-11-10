using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyCryptoMarket_MVC.Data;
using MyCryptoMarket_MVC.Models;
using MyCryptoMarket_MVC.Helper;
using System.Threading.Tasks;

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

        public async Task<IActionResult> Index()
        {
            var isSignedIn = User.Identity.IsAuthenticated;
            if (isSignedIn) 
            {
                await DbInitializer.InitializeUser(_context, User.Identity.Name);
                await DbInitializer.InitializeBalances(_context, User.Identity.Name);
            }

            return View();
        }

        [HttpPost]
        public IActionResult OnGet24hTickers(DxRequestBase args)
        {
            var query = _context.Tickers.AsQueryable();

            // if (args.DxFilter != null && args.DxFilter.Any())
            // {
            //     query = DynamicLinqExtensions.Filter(query, args.DxFilter);
            // }

            var filter = args.DxFilter.FirstOrDefault();
            if (filter != null && !string.IsNullOrEmpty(filter.Keyword))
            {
                query = query.Where(x=>x.Symbol.ToLower().Contains(filter.Keyword.ToLower()));
            }

            var totalCount =  query.Count();

            query = query.SortBy(args.OrderBy.Item1, args.OrderBy.Item2);

            if (args.Skip > 0)
            {
                query = query.Skip(args.Skip);
            }

            if (args.Take > 0)
            {
                query = query.Take(args.Take);
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
