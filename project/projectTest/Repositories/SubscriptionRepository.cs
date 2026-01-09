using Microsoft.EntityFrameworkCore;
using projectTest.Data;
using projectTest.Models.Entities;
using projectTest.Repositories.Interfaces;

namespace projectTest.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly ApplicationDbContext _context;

    public SubscriptionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Subscription?> GetByIdAsync(Guid id)
    {
        return await _context.Subscriptions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Subscription>> GetAllAsync()
    {
        return await _context.Subscriptions
            .Include(s => s.User)
            .ToListAsync();
    }

    public async Task<List<Subscription>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Subscriptions
            .Where(s => s.UserId == userId)
            .ToListAsync();
    }

    public async Task<Subscription> CreateAsync(Subscription subscription)
    {
        subscription.CreatedAt = DateTime.UtcNow;
        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();
        return subscription;
    }

    public async Task<Subscription> UpdateAsync(Subscription subscription)
    {
        subscription.UpdatedAt = DateTime.UtcNow;
        _context.Subscriptions.Update(subscription);
        await _context.SaveChangesAsync();
        return subscription;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);
        if (subscription == null)
            return false;

        _context.Subscriptions.Remove(subscription);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Subscriptions.AnyAsync(s => s.Id == id);
    }
}
