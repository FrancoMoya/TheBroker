namespace TheBroker.DTOs
{
    public class CreateStockDTO
    {
        public string SYMBOL { get; set; } = null!;

        public decimal UNIT_PRICE { get; set; }
    }

    public class StockDetailsDTO
    {
        public int ID { get; set; }
        public string SYMBOL { get; set; } = null!;
        public decimal UNIT_PRICE { get; set; }
    }
}
