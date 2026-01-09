using projectTest.Models.Entities;

namespace projectTest.Repositories.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id);
    Task<List<Booking>> GetAllAsync();
    Task<List<Booking>> GetByUserIdAsync(Guid userId);
    Task<List<Booking>> GetByClassIdAsync(Guid classId);
    Task<Booking> CreateAsync(Booking booking);
    Task<Booking> UpdateAsync(Booking booking);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    // Dapper method with transaction
    Task<bool> CreateBookingWithTransactionAsync(Booking booking, Guid userId, Guid classId);
}
