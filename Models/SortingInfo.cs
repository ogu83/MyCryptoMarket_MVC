namespace MyCryptoMarket_MVC.Models
{
    //
    // Summary:
    //     Represents a sorting parameter.
    public class SortingInfo
    {
        //
        // Summary:
        //     The data field to be used for sorting.
        public string Selector { get; set; }
        //
        // Summary:
        //     A flag indicating whether data should be sorted in a descending order.
        public bool Desc { get; set; }
    }
}
