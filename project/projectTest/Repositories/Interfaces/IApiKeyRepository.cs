using projectTest.Models.Entities;

namespace projectTest.Repositories.Interfaces;

public interface IApiKeyRepository
{
    Task<ApiKey?> GetByIdAsync(Guid id);
    Task<ApiKey?> GetByKeyAsync(string key);
    Task<List<ApiKey>> GetAllAsync();
    Task<ApiKey> CreateAsync(ApiKey apiKey);
    Task<ApiKey> UpdateAsync(ApiKey apiKey);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> IsValidAsync(string key);
}
