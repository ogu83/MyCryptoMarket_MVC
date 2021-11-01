namespace MyCryptoMarket_MVC.Models
{
    public class IdValue
    {
        public int Id { get; set; }

        public string Value { get; set; }
    }

    public class IdValueSelectable : IdValue
    {
        public bool IsSelected { get; set; }
    }
}