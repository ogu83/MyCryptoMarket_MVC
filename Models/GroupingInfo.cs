namespace MyCryptoMarket_MVC.Models
{
    //
    // Summary:
    //     Represents a grouping level to be applied to data.
    public class GroupingInfo : SortingInfo
    {
        //
        // Summary:
        //     A value that groups data in ranges of a given length or date/time period.
        public string GroupInterval { get; set; }
        //
        // Summary:
        //     A flag indicating whether the group's data objects should be returned.
        public bool? IsExpanded { get; set; }

        //
        // Summary:
        //     Returns the value of the IsExpanded field or true if this value is null.
        //
        // Returns:
        //     The value of the IsExpanded field or true if this value is null.        
    }
}
