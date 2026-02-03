using Microsoft.EntityFrameworkCore;
using PriceMonitorService.Data;
using PriceMonitorService.Models;

namespace PriceMonitorService.Repositories
{
    public class PriceRepository : IPriceRepository
    {
        private readonly AppDbContext _context;

        public PriceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddPriceRecordAsync(PriceRecord priceRecord)
        {
            _context.PriceRecords.Add(priceRecord);
            await _context.SaveChangesAsync();
        }

        public async Task<PriceRecord> GetLatestPriceAsync(int subscriptionId)
        {
            return await _context.PriceRecords
                .Where(pr => pr.SubscriptionId == subscriptionId)
                .OrderByDescending(pr => pr.RecordedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<PriceRecord> GetPreviousPriceAsync(int subscriptionId)
        {
            return await _context.PriceRecords
                .Where(pr => pr.SubscriptionId == subscriptionId)
                .OrderByDescending(pr => pr.RecordedAt)
                .Skip(1)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PriceRecord>> GetPriceHistoryAsync(int subscriptionId)
        {
            return await _context.PriceRecords
                .Where(pr => pr.SubscriptionId == subscriptionId)
                .OrderByDescending(pr => pr.RecordedAt)
                .ToListAsync();
        }
    }
}
