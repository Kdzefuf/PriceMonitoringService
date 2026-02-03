using PriceMonitorService.Models;
using PriceMonitorService.Repositories;

namespace PriceMonitorService.Services
{
    public class PriceMonitoringService : IPriceMonitoringService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IPriceRepository _priceRepository;
        private readonly IApartmentPriceService _webScraperService;
        private readonly IEmailNotificationService _emailService;

        public PriceMonitoringService(
            ISubscriptionRepository subscriptionRepository,
            IPriceRepository priceRepository,
            IApartmentPriceService webScraperService,
            IEmailNotificationService emailService)
        {
            _subscriptionRepository = subscriptionRepository;
            _priceRepository = priceRepository;
            _webScraperService = webScraperService;
            _emailService = emailService;
        }

        public async Task CheckAllPricesAsync()
        {
            var activeSubscriptions = await _subscriptionRepository.GetAllActiveAsync();

            foreach (var subscription in activeSubscriptions)
            {
                await CheckPriceForSubscriptionAsync(subscription);
            }
        }

        private async Task CheckPriceForSubscriptionAsync(Subscription subscription)
        {
            var currentPrice = await _webScraperService.GetPriceFromListingAsync(subscription.ListingUrl);

            if (currentPrice.HasValue)
            {
                var latestPriceRecord = await _priceRepository.GetLatestPriceAsync(subscription.Id);

                if (latestPriceRecord == null || latestPriceRecord.Price != currentPrice.Value)
                {
                    var newPriceRecord = new PriceRecord
                    {
                        SubscriptionId = subscription.Id,
                        Price = currentPrice.Value,
                        RecordedAt = DateTime.UtcNow,
                    };

                    await _priceRepository.AddPriceRecordAsync(newPriceRecord);

                    if (latestPriceRecord != null)
                    {
                        await _emailService.SendPriceChangeNotificationAsync(
                            subscription.Email,
                            subscription.ListingUrl,
                            latestPriceRecord.Price,
                            currentPrice.Value
                        );
                    }
                }
            }
        }

        public async Task<decimal?> GetCurrentPriceAsync(string listingUrl)
        {
            return await _webScraperService.GetPriceFromListingAsync(listingUrl);
        }

        public async Task<ListingPriceInfo?> GetCurrentPriceInfoForSubscriptionAsync(string listingUrl)
        {
            var subscription = await _subscriptionRepository.GetByUrlAsync(listingUrl);

            if (subscription == null)
            {
                return null;
            }

            var latestPrice = await _priceRepository.GetLatestPriceAsync(subscription.Id);
            var previousPriceRecord = await _priceRepository.GetPreviousPriceAsync(subscription.Id);

            var currentPrice = await _webScraperService.GetPriceFromListingAsync(listingUrl);

            return new ListingPriceInfo
            {
                ListingUrl = listingUrl,
                CurrentPrice = currentPrice ?? (latestPrice?.Price ?? 0),
                PreviousPrice = previousPriceRecord?.Price,
                LastChecked = DateTime.UtcNow
            };
        }
    }
}