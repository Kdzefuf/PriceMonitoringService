using PriceMonitorService.Models;

namespace PriceMonitorService.Services
{
    public interface IPriceMonitoringService
    {
        Task CheckAllPricesAsync();

        Task<decimal?> GetCurrentPriceAsync(string listingUrl);

        Task<ListingPriceInfo?> GetCurrentPriceInfoForSubscriptionAsync(string listingUrl);
    }
}