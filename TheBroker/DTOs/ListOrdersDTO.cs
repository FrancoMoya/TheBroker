using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TheBroker.DTOs
{
    public class ListOrdersDTO
    {
        [Key]
        public int TX_NUMBER { get; set; }
        public DateTime ORDER_DATE { get; set; }
        public string ACTION { get; set; }
        public string STATUS { get; set; }
        public string SYMBOL { get; set; }
        public int QUANTITY { get; set; }
    }
    public class ListOrdersExecutedDTO
    {
        [Key]
        public int TX_NUMBER { get; set; }
        public DateTime ORDER_DATE { get; set; }
        public string ACTION { get; set; }
        public string STATUS { get; set; }
        public string SYMBOL { get; set; }
        public int QUANTITY { get; set; }
        public decimal MONTO_NETO { get; set; } 
    }
}