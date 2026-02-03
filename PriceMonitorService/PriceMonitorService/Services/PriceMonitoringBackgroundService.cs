namespace PriceMonitorService.Services
{
    public class PriceMonitoringBackgroundService : BackgroundService
    {
        private readonly ILogger<PriceMonitoringBackgroundService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;

        public PriceMonitoringBackgroundService(
            ILogger<PriceMonitoringBackgroundService> logger,
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Запуск службы мониторинга цен");

            var checkIntervalMinutes = _configuration.GetValue<int>("PriceMonitoring:CheckIntervalMinutes", 60);
            var checkInterval = TimeSpan.FromMinutes(checkIntervalMinutes);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Начало проверки цены в {time}", DateTimeOffset.Now);

                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var priceMonitoringService = scope.ServiceProvider.GetRequiredService<IPriceMonitoringService>();
                        await priceMonitoringService.CheckAllPricesAsync();
                    }

                    _logger.LogInformation("Проверка цены завершена в {time}", DateTimeOffset.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при мониторинге цен");
                }

                await Task.Delay(checkInterval, stoppingToken);
            }
        }
    }
}
