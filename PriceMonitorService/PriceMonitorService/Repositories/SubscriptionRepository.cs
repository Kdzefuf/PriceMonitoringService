using Microsoft.EntityFrameworkCore;
using PriceMonitorService.Data;
using PriceMonitorService.Models;

namespace PriceMonitorService.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly AppDbContext _context;

        public SubscriptionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Subscription> AddAsync(Subscription subscription)
        {
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task<Subscription> GetByIdAsync(int id)
        {
            return await _context.Subscriptions.FindAsync(id);
        }

        public async Task<Subscription> GetByUrlAsync(string url)
        {
            return await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.ListingUrl == url);
        }

        public async Task<IEnumerable<Subscription>> GetAllActiveAsync()
        {
            return await _context.Subscriptions.ToListAsync();
        }

        public async Task UpdateAsync(Subscription subscription)
        {
            _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription != null)
            {
                _context.Subscriptions.Remove(subscription);
                await _context.SaveChangesAsync();
            }
        }
    }
}
