namespace PriceMonitorService.Models
{
    public class ListingPriceInfo
    {
        public string ListingUrl { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal? PreviousPrice { get; set; }
        public DateTime LastChecked { get; set; }
    }
}
