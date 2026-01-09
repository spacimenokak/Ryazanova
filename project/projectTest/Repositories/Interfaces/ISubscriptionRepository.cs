using projectTest.Models.Entities;

namespace projectTest.Repositories.Interfaces;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetByIdAsync(Guid id);
    Task<List<Subscription>> GetAllAsync();
    Task<List<Subscription>> GetByUserIdAsync(Guid userId);
    Task<Subscription> CreateAsync(Subscription subscription);
    Task<Subscription> UpdateAsync(Subscription subscription);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
