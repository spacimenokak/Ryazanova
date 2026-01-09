using projectTest.Models.DTO;

namespace projectTest.Services.Interfaces;

public interface ISubscriptionService
{
    Task<SubscriptionDto?> GetByIdAsync(Guid id, string? userRole, Guid? userId);
    Task<List<SubscriptionDto>> GetAllAsync(string? userRole);
    Task<List<SubscriptionDto>> GetByUserIdAsync(Guid userId, string? userRole, Guid? currentUserId);
    Task<SubscriptionDto> CreateAsync(CreateSubscriptionDto dto, string? userRole);
    Task<SubscriptionDto> UpdateAsync(Guid id, UpdateSubscriptionDto dto, string? userRole, Guid? userId);
    Task<bool> DeleteAsync(Guid id, string? userRole);
}
