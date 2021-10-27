namespace MyCryptoMarket_MVC.Models
{
    public class FilterOptions : PagingOptions
    {
        public long? ContactId { get; set; }

        public long? CompanyId { get; set; }

        public string Search { get; set; }

        public string OrderBy { get; set; }

        public bool IsDescending { get; set; }
    }
}
