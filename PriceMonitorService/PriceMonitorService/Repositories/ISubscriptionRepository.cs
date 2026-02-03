using PriceMonitorService.Models;

namespace PriceMonitorService.Repositories
{
    public interface ISubscriptionRepository
    {
        Task<Subscription> AddAsync(Subscription subscription);
        Task<Subscription> GetByIdAsync(int id);
        Task<Subscription> GetByUrlAsync(string url);
        Task<IEnumerable<Subscription>> GetAllActiveAsync();
        Task UpdateAsync(Subscription subscription);
        Task DeleteAsync(int id);
    }
}
