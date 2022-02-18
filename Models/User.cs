using System.ComponentModel.DataAnnotations;

namespace MyCryptoMarket_MVC.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
