using Binance.Net.Enums;

namespace MyCryptoMarket_MVC.Models
{
    public class Symbol
    {
        public string Name { get; set; }

        public SymbolStatus Status { get; set; }

        public string BaseAsset { get; set; }
        
        public int BaseAssetPrecision { get; set; }
    }
}
