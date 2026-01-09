using projectTest.Models.DTO;

namespace projectTest.Services.Interfaces;

public interface IInstructorService
{
    Task<InstructorDto?> GetByIdAsync(Guid id);
    Task<List<InstructorDto>> GetAllAsync();
    Task<InstructorDto> CreateAsync(CreateInstructorDto dto, string? userRole);
    Task<InstructorDto> UpdateAsync(Guid id, UpdateInstructorDto dto, string? userRole);
    Task<bool> DeleteAsync(Guid id, string? userRole);
}
