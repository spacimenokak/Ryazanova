using projectTest.Models.DTO;

namespace projectTest.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<string> GenerateTokenAsync(UserDto user);
}
