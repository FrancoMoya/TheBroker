using System;
using System.ComponentModel.DataAnnotations;

namespace TheBroker.DTOs
{
    public class CreateOrdersDTO
    {
        public string ACTION { get; set; }
        public string STATUS { get; set; }
        public int ID_SYMBOL { get; set; }
        public int QUANTITY { get; set; }
    }
    public class OrderDetailsDTO
    {
        public int TX_NUMBER { get; set; }
        public DateTime ORDER_DATE { get; set; }
        public string ACTION { get; set; }
        public string STATUS { get; set; }
        public int ID_SYMBOL { get; set; }
        public int QUANTITY { get; set; }
    }
    public class CreateOrderExecutedDTO
    {
        public string ACTION { get; set; }
        public int ID_SYMBOL { get; set; }
        public int QUANTITY { get; set; }
    }
}
