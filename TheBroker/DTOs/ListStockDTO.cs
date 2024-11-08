using System.ComponentModel.DataAnnotations;

namespace TheBroker.DTOs
{
    public class ListStockDTO
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string SYMBOL { get; set; }
        [Required]
        public decimal UNIT_PRICE { get; set; }
    }
}
