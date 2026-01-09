using projectTest.Models.DTO;

namespace projectTest.Services.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid id, string? userRole, Guid? userId);
    Task<List<UserDto>> GetAllAsync(string? userRole);
    Task<UserDto> CreateAsync(CreateUserDto dto, string? userRole);
    Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto, string? userRole, Guid? userId);
    Task<bool> DeleteAsync(Guid id, string? userRole);
}
