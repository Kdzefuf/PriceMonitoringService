namespace PriceMonitorService.Services
{
    public interface IEmailNotificationService
    {
        Task SendPriceChangeNotificationAsync(string email, string listingUrl, decimal oldPrice, decimal newPrice);
    }
}
