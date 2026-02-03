namespace PriceMonitorService.Models
{
    public class PriceRecord
    {
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public decimal Price { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
