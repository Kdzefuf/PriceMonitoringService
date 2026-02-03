using Microsoft.AspNetCore.Mvc;
using PriceMonitorService.Models;
using PriceMonitorService.Repositories;
using PriceMonitorService.Services;

namespace PriceMonitorService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PriceController : ControllerBase
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IPriceMonitoringService _priceMonitoringService;

        public PriceController(
            ISubscriptionRepository subscriptionRepository,
            IPriceMonitoringService priceMonitoringService)
        {
            _subscriptionRepository = subscriptionRepository;
            _priceMonitoringService = priceMonitoringService;
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentPrices()
        {
            var activeSubscriptions = await _subscriptionRepository.GetAllActiveAsync();
            var priceInfos = new List<ListingPriceInfo>();

            foreach (var subscription in activeSubscriptions)
            {
                var priceInfo = await _priceMonitoringService.GetCurrentPriceInfoForSubscriptionAsync(subscription.ListingUrl);
                if (priceInfo != null)
                {
                    priceInfos.Add(priceInfo);
                }
            }

            return Ok(priceInfos);
        }


        [HttpGet("history/{subscriptionId}")]
        public async Task<IActionResult> GetPriceHistory(int subscriptionId)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(subscriptionId);

            if (subscription == null)
            {
                return NotFound(new { message = "Подписка не найдена" });
            }

            var priceRepository = HttpContext.RequestServices.GetRequiredService<IPriceRepository>();
            var priceHistory = await priceRepository.GetPriceHistoryAsync(subscriptionId);

            return Ok(priceHistory);
        }
    }
}
