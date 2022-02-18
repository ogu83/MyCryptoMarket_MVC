using System.Collections.Generic;

namespace MyCryptoMarket_MVC.Models
{
    public class FilterOptionsEx : FilterOptions
    {
        private string _orderBy = "Id";

        public new string OrderBy
        {
            get
            {
                return _orderBy;
            }

            set
            {
                if (value == "Actions")
                {
                    return;
                }

                if (value != _orderBy)
                {
                    _orderBy = value;

                    if (_orderBy.EndsWith("Str"))
                    {
                        _orderBy = _orderBy.Substring(0, _orderBy.Length - 3);
                    }
                }
            }
        }

        public List<DxFilter> DxFilter { get; set; } = new List<DxFilter>();
    }
}