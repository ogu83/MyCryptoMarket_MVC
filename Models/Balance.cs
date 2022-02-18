using System.ComponentModel.DataAnnotations;

namespace MyCryptoMarket_MVC.Models
{
    public class Balance
    {        
        [Key]
        public int Id { get; set; }

        public int User_Id { get; set; }

        public string AssetName { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal AmountInUse { get; set; }

        public decimal AvailableAmount { get { return TotalAmount - AmountInUse; } }
    }    
}