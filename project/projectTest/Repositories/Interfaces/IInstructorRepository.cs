using projectTest.Models.Entities;

namespace projectTest.Repositories.Interfaces;

public interface IInstructorRepository
{
    Task<Instructor?> GetByIdAsync(Guid id);
    Task<List<Instructor>> GetAllAsync();
    Task<Instructor> CreateAsync(Instructor instructor);
    Task<Instructor> UpdateAsync(Instructor instructor);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
