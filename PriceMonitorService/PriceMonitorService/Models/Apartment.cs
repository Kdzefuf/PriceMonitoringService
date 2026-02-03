namespace PriceMonitorService.Models
{
    public class ApartmentApiResponse
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public List<Pricing> Pricings { get; set; }
    }
}
