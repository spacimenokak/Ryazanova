using projectTest.Models.DTO;
using projectTest.Models.Entities;
using projectTest.Repositories.Interfaces;
using projectTest.Services.Interfaces;

namespace projectTest.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IClassRepository _classRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<BookingService> _logger;
    private static readonly Dictionary<string, Guid> _idempotencyKeys = new();

    public BookingService(
        IBookingRepository bookingRepository,
        IClassRepository classRepository,
        IUserRepository userRepository,
        ICacheService cacheService,
        ILogger<BookingService> logger)
    {
        _bookingRepository = bookingRepository;
        _classRepository = classRepository;
        _userRepository = userRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<BookingDto?> GetByIdAsync(Guid id, string? userRole, Guid? userId)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
            return null;

        // Users can only view their own bookings unless they are Admin or Manager
        if (userRole != "Admin" && userRole != "Manager" && userId != booking.UserId)
        {
            throw new UnauthorizedAccessException("You don't have permission to view this booking");
        }

        return MapToDto(booking);
    }

    public async Task<List<BookingDto>> GetAllAsync(string? userRole)
    {
        // Only Admin and Manager can view all bookings
        if (userRole != "Admin" && userRole != "Manager")
        {
            throw new UnauthorizedAccessException("You don't have permission to view all bookings");
        }

        var bookings = await _bookingRepository.GetAllAsync();
        return bookings.Select(MapToDto).ToList();
    }

    public async Task<List<BookingDto>> GetByUserIdAsync(Guid userId, string? userRole, Guid? currentUserId)
    {
        // Users can only view their own bookings unless they are Admin or Manager
        if (userRole != "Admin" && userRole != "Manager" && currentUserId != userId)
        {
            throw new UnauthorizedAccessException("You don't have permission to view these bookings");
        }

        var bookings = await _bookingRepository.GetByUserIdAsync(userId);
        return bookings.Select(MapToDto).ToList();
    }

    public async Task<BookingDto> CreateAsync(CreateBookingDto dto, Guid userId, string? idempotencyKey)
    {
        // Check idempotency
        if (!string.IsNullOrEmpty(idempotencyKey))
        {
            if (_idempotencyKeys.TryGetValue(idempotencyKey, out var existingBookingId))
            {
                var existingBooking = await _bookingRepository.GetByIdAsync(existingBookingId);
                if (existingBooking != null)
                {
                    _logger.LogInformation("Idempotent booking request: {IdempotencyKey} -> {BookingId}", idempotencyKey, existingBookingId);
                    return MapToDto(existingBooking);
                }
            }
        }

        var classEntity = await _classRepository.GetByIdAsync(dto.ClassId);
        if (classEntity == null)
            throw new KeyNotFoundException("Class not found");

        if (classEntity.CurrentParticipants >= classEntity.MaxParticipants)
        {
            throw new InvalidOperationException("Class is full");
        }

        // Use Dapper transaction method
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ClassId = dto.ClassId,
            BookingDate = DateTime.UtcNow,
            Status = "Active"
        };

        var success = await _bookingRepository.CreateBookingWithTransactionAsync(booking, userId, dto.ClassId);
        if (!success)
        {
            throw new InvalidOperationException("Failed to create booking. Class may be full or booking already exists.");
        }

        // Store idempotency key
        if (!string.IsNullOrEmpty(idempotencyKey))
        {
            _idempotencyKeys[idempotencyKey] = booking.Id;
        }

        var createdBooking = await _bookingRepository.GetByIdAsync(booking.Id);
        if (createdBooking == null)
            throw new InvalidOperationException("Failed to retrieve created booking");

        _logger.LogInformation("Booking created: {BookingId} for user {UserId} to class {ClassId}", booking.Id, userId, dto.ClassId);

        // Invalidate cache
        await _cacheService.RemoveByPatternAsync("classes:*");
        await _cacheService.RemoveAsync($"class:{dto.ClassId}");

        return MapToDto(createdBooking);
    }

    public async Task<BookingDto> UpdateAsync(Guid id, UpdateBookingDto dto, string? userRole, Guid? userId)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
            throw new KeyNotFoundException("Booking not found");

        // Users can only update their own bookings, unless they are Admin or Manager
        if (userRole != "Admin" && userRole != "Manager" && userId != booking.UserId)
        {
            throw new UnauthorizedAccessException("You don't have permission to update this booking");
        }

        if (dto.Status != null)
            booking.Status = dto.Status;

        var updatedBooking = await _bookingRepository.UpdateAsync(booking);
        _logger.LogInformation("Booking updated: {BookingId}", booking.Id);

        // Invalidate cache
        await _cacheService.RemoveAsync($"class:{booking.ClassId}");

        return MapToDto(updatedBooking);
    }

    public async Task<bool> DeleteAsync(Guid id, string? userRole, Guid? userId)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
            return false;

        // Only Admin can delete bookings, or users can delete their own
        if (userRole != "Admin" && userId != booking.UserId)
        {
            throw new UnauthorizedAccessException("You don't have permission to delete this booking");
        }

        var result = await _bookingRepository.DeleteAsync(id);
        if (result)
        {
            _logger.LogInformation("Booking deleted: {Id}", id);
            // Invalidate cache
            await _cacheService.RemoveAsync($"class:{booking.ClassId}");
        }

        return result;
    }

    private static BookingDto MapToDto(Booking booking)
    {
        return new BookingDto
        {
            Id = booking.Id,
            UserId = booking.UserId,
            User = booking.User != null ? new UserDto
            {
                Id = booking.User.Id,
                Email = booking.User.Email,
                FirstName = booking.User.FirstName,
                LastName = booking.User.LastName,
                PhoneNumber = booking.User.PhoneNumber,
                Role = booking.User.Role,
                CreatedAt = booking.User.CreatedAt
            } : null,
            ClassId = booking.ClassId,
            Class = booking.Class != null ? new ClassDto
            {
                Id = booking.Class.Id,
                Name = booking.Class.Name,
                Description = booking.Class.Description,
                StartTime = booking.Class.StartTime,
                EndTime = booking.Class.EndTime,
                MaxParticipants = booking.Class.MaxParticipants,
                CurrentParticipants = booking.Class.CurrentParticipants,
                Price = booking.Class.Price,
                InstructorId = booking.Class.InstructorId,
                CreatedAt = booking.Class.CreatedAt
            } : null,
            BookingDate = booking.BookingDate,
            Status = booking.Status,
            CreatedAt = booking.CreatedAt
        };
    }
}
