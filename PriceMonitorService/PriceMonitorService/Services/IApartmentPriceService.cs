namespace PriceMonitorService.Services
{
    public interface IApartmentPriceService
    {
        Task<decimal?> GetPriceFromListingAsync(string listingUrl);
    }
}
