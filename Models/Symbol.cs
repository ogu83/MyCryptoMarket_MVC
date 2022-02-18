using System.ComponentModel.DataAnnotations;
using Binance.Net.Enums;

namespace MyCryptoMarket_MVC.Models
{
    public class Symbol
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public SymbolStatus Status { get; set; }

        public string BaseAsset { get; set; }

        public int BaseAssetPrecision { get; set; }

        public string QuoteAsset { get;  set; }

        public int QuotePrecision { get;  set; }

        public int BaseCommissionPrecision { get;  set; }

        public int QuoteCommissionPrecision { get;  set; }
    }
}
