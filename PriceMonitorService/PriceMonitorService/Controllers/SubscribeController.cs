using Microsoft.AspNetCore.Mvc;
using PriceMonitorService.Models;
using PriceMonitorService.Repositories;
using PriceMonitorService.Services;

namespace PriceMonitorService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscribeController : ControllerBase
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IPriceMonitoringService _priceMonitoringService;

        public SubscribeController(
            ISubscriptionRepository subscriptionRepository,
            IPriceMonitoringService priceMonitoringService)
        {
            _subscriptionRepository = subscriptionRepository;
            _priceMonitoringService = priceMonitoringService;
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe([FromBody] SubscriptionRequest request)
        {
            if (!request.IsValid())
            {
                return BadRequest(new { message = "Неверный формат ссылки или email" });
            }

            var existingSubscription = await _subscriptionRepository.GetByUrlAsync(request.ListingUrl);

            if (existingSubscription != null)
            {
                return BadRequest(new { message = "Подписка на этот объект уже существует" });
            }

            var currentPrice = await _priceMonitoringService.GetCurrentPriceAsync(request.ListingUrl);

            if (currentPrice == null || currentPrice == 0)
            {
                return BadRequest(new
                {
                    message =
                    "Не удалось получить цену по указанной ссылке. Проверьте корректность URL."
                });
            }

            var subscription = new Subscription
            {
                ListingUrl = request.ListingUrl,
                Email = request.Email,
                CreatedAt = DateTime.UtcNow,
            };

            await _subscriptionRepository.AddAsync(subscription);

            var priceRepository = HttpContext.RequestServices.GetRequiredService<IPriceRepository>();
            await priceRepository.AddPriceRecordAsync(new PriceRecord
            {
                SubscriptionId = subscription.Id,
                Price = currentPrice.Value,
                RecordedAt = DateTime.UtcNow,
            });

            return Ok(new
            {
                message = "Подписка успешно создана",
                subscriptionId = subscription.Id,
                currentPrice = currentPrice.Value,
                apartmentUrl = request.ListingUrl
            });
        }
    }
}
