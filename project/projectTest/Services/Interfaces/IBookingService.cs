using projectTest.Models.DTO;

namespace projectTest.Services.Interfaces;

public interface IBookingService
{
    Task<BookingDto?> GetByIdAsync(Guid id, string? userRole, Guid? userId);
    Task<List<BookingDto>> GetAllAsync(string? userRole);
    Task<List<BookingDto>> GetByUserIdAsync(Guid userId, string? userRole, Guid? currentUserId);
    Task<BookingDto> CreateAsync(CreateBookingDto dto, Guid userId, string? idempotencyKey);
    Task<BookingDto> UpdateAsync(Guid id, UpdateBookingDto dto, string? userRole, Guid? userId);
    Task<bool> DeleteAsync(Guid id, string? userRole, Guid? userId);
}
