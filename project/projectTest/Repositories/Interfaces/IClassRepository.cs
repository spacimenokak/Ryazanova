using projectTest.Models.Entities;

namespace projectTest.Repositories.Interfaces;

public interface IClassRepository
{
    Task<Class?> GetByIdAsync(Guid id);
    Task<List<Class>> GetAllAsync();
    Task<List<Class>> GetFilteredAsync(string? search, Guid? instructorId, DateTime? startDate, DateTime? endDate, decimal? minPrice, decimal? maxPrice);
    Task<int> GetTotalCountAsync(string? search, Guid? instructorId, DateTime? startDate, DateTime? endDate, decimal? minPrice, decimal? maxPrice);
    Task<Class> CreateAsync(Class classEntity);
    Task<Class> UpdateAsync(Class classEntity);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
