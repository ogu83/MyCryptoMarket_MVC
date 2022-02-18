namespace MyCryptoMarket_MVC.Models
{
    public class PagingOptions
    {
        public int? PageNo { get; set; }

        public int? PageSize { get; set; }

        public int? Skip { get; set; }

        public int? Take { get; set; }
    }
}
