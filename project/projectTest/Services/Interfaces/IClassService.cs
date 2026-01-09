using projectTest.Models.DTO;

namespace projectTest.Services.Interfaces;

public interface IClassService
{
    Task<ClassDto?> GetByIdAsync(Guid id);
    Task<PagedResponseDto<ClassDto>> GetFilteredAsync(ClassFilterDto filter, int page, int pageSize);
    Task<ClassDto> CreateAsync(CreateClassDto dto, string? userRole);
    Task<ClassDto> UpdateAsync(Guid id, UpdateClassDto dto, string? userRole);
    Task<bool> DeleteAsync(Guid id, string? userRole);
}
