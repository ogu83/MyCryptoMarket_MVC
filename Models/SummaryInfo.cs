namespace MyCryptoMarket_MVC.Models
{
    //
    // Summary:
    //     Represents a group or total summary definition.
    public class SummaryInfo
    {
        //
        // Summary:
        //     The data field to be used for calculating the summary.
        public string Selector { get; set; }
        //
        // Summary:
        //     An aggregate function: "sum", "min", "max", "avg", or "count".
        public string SummaryType { get; set; }
    }
}
