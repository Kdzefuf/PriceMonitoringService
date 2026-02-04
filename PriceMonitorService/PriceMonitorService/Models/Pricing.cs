namespace PriceMonitorService.Models
{
    public class Pricing
    {
        public int Id { get; set; }
        public decimal? PriceBase { get; set; }
        public decimal? Price { get; set; }
        public int? MortgageId { get; set; }
    }
}
