using PriceMonitorService.Models;

namespace PriceMonitorService.Repositories
{
    public interface IPriceRepository
    {
        Task AddPriceRecordAsync(PriceRecord priceRecord);
        Task<PriceRecord> GetLatestPriceAsync(int subscriptionId);
        Task<PriceRecord> GetPreviousPriceAsync(int subscriptionId);
        Task<IEnumerable<PriceRecord>> GetPriceHistoryAsync(int subscriptionId);
    }
}
