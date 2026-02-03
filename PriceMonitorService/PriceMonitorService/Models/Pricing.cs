namespace PriceMonitorService.Models
{
    public class Pricing
    {
        public int Id { get; set; }
        public string PriceBase { get; set; }
        public string Price { get; set; }
        public int? MortgageId { get; set; }
    }
}
