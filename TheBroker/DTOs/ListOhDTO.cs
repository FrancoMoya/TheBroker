namespace TheBroker.DTOs
{
    public class ListOhDTO
    {
        public int ID { get; set; }
        public int TX_NUMBER { get; set; }
        public DateTime? CHANGED_DATE { get; set; }
        public string ACTION { get; set; } = null!;
        public string STATUS_BEFORE { get; set; } = null!;
        public string? STATUS_AFTER { get; set; }
        public string SYMBOL { get; set; }
        public string DESCRIPTION { get; set; } = null!;
    }
}
