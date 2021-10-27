namespace MyCryptoMarket_MVC.Models
{
    public class DxGridRequest
    {
        public int skip { get; set; }

        public int take  { get; set; }

        public object sort { get; set; }
    }
}
