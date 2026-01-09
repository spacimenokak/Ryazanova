using projectTest.Models.DTO;
using projectTest.Models.Entities;
using projectTest.Repositories.Interfaces;
using projectTest.Services.Interfaces;

namespace projectTest.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserDto?> GetByIdAsync(Guid id, string? userRole, Guid? userId)
    {
        // Users can only view their own data unless they are Admin or Manager
        if (userRole != "Admin" && userRole != "Manager" && userId != id)
        {
            throw new UnauthorizedAccessException("You don't have permission to view this user");
        }

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return null;

        return MapToDto(user);
    }

    public async Task<List<UserDto>> GetAllAsync(string? userRole)
    {
        // Only Admin and Manager can view all users
        if (userRole != "Admin" && userRole != "Manager")
        {
            throw new UnauthorizedAccessException("You don't have permission to view all users");
        }

        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDto).ToList();
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto, string? userRole)
    {
        // Only Admin can create users
        if (userRole != "Admin")
        {
            throw new UnauthorizedAccessException("Only administrators can create users");
        }

        var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            PasswordHash = AuthService.HashPassword(dto.Password),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            Role = dto.Role
        };

        var createdUser = await _userRepository.CreateAsync(user);
        _logger.LogInformation("User created: {Email}", createdUser.Email);

        return MapToDto(createdUser);
    }

    public async Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto, string? userRole, Guid? userId)
    {
        // Users can only update their own data, unless they are Admin
        if (userRole != "Admin" && userId != id)
        {
            throw new UnauthorizedAccessException("You don't have permission to update this user");
        }

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        // Only Admin can change roles
        if (dto.Role != null && userRole != "Admin")
        {
            throw new UnauthorizedAccessException("Only administrators can change user roles");
        }

        if (dto.FirstName != null)
            user.FirstName = dto.FirstName;
        if (dto.LastName != null)
            user.LastName = dto.LastName;
        if (dto.PhoneNumber != null)
            user.PhoneNumber = dto.PhoneNumber;
        if (dto.Role != null && userRole == "Admin")
            user.Role = dto.Role;

        var updatedUser = await _userRepository.UpdateAsync(user);
        _logger.LogInformation("User updated: {Email}", updatedUser.Email);

        return MapToDto(updatedUser);
    }

    public async Task<bool> DeleteAsync(Guid id, string? userRole)
    {
        // Only Admin can delete users
        if (userRole != "Admin")
        {
            throw new UnauthorizedAccessException("Only administrators can delete users");
        }

        var result = await _userRepository.DeleteAsync(id);
        if (result)
        {
            _logger.LogInformation("User deleted: {Id}", id);
        }

        return result;
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }
}
