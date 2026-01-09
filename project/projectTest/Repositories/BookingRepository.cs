using Dapper;
using Microsoft.EntityFrameworkCore;
using projectTest.Data;
using projectTest.Models.Entities;
using projectTest.Repositories.Interfaces;
using System.Data;

namespace projectTest.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IDbConnectionFactory _connectionFactory;

    public BookingRepository(ApplicationDbContext context, IDbConnectionFactory connectionFactory)
    {
        _context = context;
        _connectionFactory = connectionFactory;
    }

    public async Task<Booking?> GetByIdAsync(Guid id)
    {
        return await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Class)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<List<Booking>> GetAllAsync()
    {
        return await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Class)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Bookings
            .Include(b => b.Class)
            .Where(b => b.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetByClassIdAsync(Guid classId)
    {
        return await _context.Bookings
            .Include(b => b.User)
            .Where(b => b.ClassId == classId)
            .ToListAsync();
    }

    public async Task<Booking> CreateAsync(Booking booking)
    {
        booking.CreatedAt = DateTime.UtcNow;
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task<Booking> UpdateAsync(Booking booking)
    {
        booking.UpdatedAt = DateTime.UtcNow;
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
            return false;

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Bookings.AnyAsync(b => b.Id == id);
    }

    // Dapper method with transaction - creates booking and updates class participants count
    public async Task<bool> CreateBookingWithTransactionAsync(Booking booking, Guid userId, Guid classId)
    {
        using var connection = _connectionFactory.GetConnection();
        using var transaction = connection.BeginTransaction();
        
        try
        {
            // Check if class exists and has available spots
            var classQuery = @"
                SELECT ""Id"", ""MaxParticipants"", ""CurrentParticipants""
                FROM ""Classes""
                WHERE ""Id"" = @ClassId
                FOR UPDATE";
            
            var classEntity = await connection.QueryFirstOrDefaultAsync<dynamic>(
                classQuery, 
                new { ClassId = classId }, 
                transaction);

            if (classEntity == null)
            {
                transaction.Rollback();
                return false;
            }

            if (classEntity.CurrentParticipants >= classEntity.MaxParticipants)
            {
                transaction.Rollback();
                return false;
            }

            // Check if user already has a booking for this class
            var existingBookingQuery = @"
                SELECT COUNT(*) 
                FROM ""Bookings""
                WHERE ""UserId"" = @UserId AND ""ClassId"" = @ClassId AND ""Status"" = 'Active'";
            
            var existingCount = await connection.QuerySingleAsync<int>(
                existingBookingQuery,
                new { UserId = userId, ClassId = classId },
                transaction);

            if (existingCount > 0)
            {
                transaction.Rollback();
                return false;
            }

            // Create booking
            var bookingId = Guid.NewGuid();
            var insertBookingQuery = @"
                INSERT INTO ""Bookings"" (""Id"", ""UserId"", ""ClassId"", ""BookingDate"", ""Status"", ""CreatedAt"")
                VALUES (@Id, @UserId, @ClassId, @BookingDate, @Status, @CreatedAt)";
            
            await connection.ExecuteAsync(
                insertBookingQuery,
                new
                {
                    Id = bookingId,
                    UserId = userId,
                    ClassId = classId,
                    BookingDate = booking.BookingDate,
                    Status = booking.Status,
                    CreatedAt = DateTime.UtcNow
                },
                transaction);

            // Update class participants count
            var updateClassQuery = @"
                UPDATE ""Classes""
                SET ""CurrentParticipants"" = ""CurrentParticipants"" + 1,
                    ""UpdatedAt"" = @UpdatedAt
                WHERE ""Id"" = @ClassId";
            
            await connection.ExecuteAsync(
                updateClassQuery,
                new { ClassId = classId, UpdatedAt = DateTime.UtcNow },
                transaction);

            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
