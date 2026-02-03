namespace PriceMonitorService.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public string ListingUrl { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
