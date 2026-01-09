using projectTest.Models.DTO;
using projectTest.Models.Entities;
using projectTest.Repositories.Interfaces;
using projectTest.Services.Interfaces;

namespace projectTest.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<SubscriptionService> _logger;

    public SubscriptionService(
        ISubscriptionRepository subscriptionRepository,
        IUserRepository userRepository,
        ILogger<SubscriptionService> logger)
    {
        _subscriptionRepository = subscriptionRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<SubscriptionDto?> GetByIdAsync(Guid id, string? userRole, Guid? userId)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(id);
        if (subscription == null)
            return null;

        // Users can only view their own subscriptions unless they are Admin or Manager
        if (userRole != "Admin" && userRole != "Manager" && userId != subscription.UserId)
        {
            throw new UnauthorizedAccessException("You don't have permission to view this subscription");
        }

        return MapToDto(subscription);
    }

    public async Task<List<SubscriptionDto>> GetAllAsync(string? userRole)
    {
        // Only Admin and Manager can view all subscriptions
        if (userRole != "Admin" && userRole != "Manager")
        {
            throw new UnauthorizedAccessException("You don't have permission to view all subscriptions");
        }

        var subscriptions = await _subscriptionRepository.GetAllAsync();
        return subscriptions.Select(MapToDto).ToList();
    }

    public async Task<List<SubscriptionDto>> GetByUserIdAsync(Guid userId, string? userRole, Guid? currentUserId)
    {
        // Users can only view their own subscriptions unless they are Admin or Manager
        if (userRole != "Admin" && userRole != "Manager" && currentUserId != userId)
        {
            throw new UnauthorizedAccessException("You don't have permission to view these subscriptions");
        }

        var subscriptions = await _subscriptionRepository.GetByUserIdAsync(userId);
        return subscriptions.Select(MapToDto).ToList();
    }

    public async Task<SubscriptionDto> CreateAsync(CreateSubscriptionDto dto, string? userRole)
    {
        // Only Admin and Manager can create subscriptions
        if (userRole != "Admin" && userRole != "Manager")
        {
            throw new UnauthorizedAccessException("You don't have permission to create subscriptions");
        }

        if (!await _userRepository.ExistsAsync(dto.UserId))
        {
            throw new KeyNotFoundException("User not found");
        }

        if (dto.StartDate >= dto.EndDate)
        {
            throw new ArgumentException("Start date must be before end date");
        }

        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            Type = dto.Type,
            Price = dto.Price,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsActive = true
        };

        var createdSubscription = await _subscriptionRepository.CreateAsync(subscription);
        _logger.LogInformation("Subscription created: {Id} for user {UserId}", createdSubscription.Id, dto.UserId);

        return MapToDto(createdSubscription);
    }

    public async Task<SubscriptionDto> UpdateAsync(Guid id, UpdateSubscriptionDto dto, string? userRole, Guid? userId)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(id);
        if (subscription == null)
            throw new KeyNotFoundException("Subscription not found");

        // Only Admin and Manager can update subscriptions
        if (userRole != "Admin" && userRole != "Manager")
        {
            throw new UnauthorizedAccessException("You don't have permission to update subscriptions");
        }

        if (dto.IsActive.HasValue)
            subscription.IsActive = dto.IsActive.Value;
        if (dto.EndDate.HasValue)
            subscription.EndDate = dto.EndDate.Value;

        var updatedSubscription = await _subscriptionRepository.UpdateAsync(subscription);
        _logger.LogInformation("Subscription updated: {Id}", subscription.Id);

        return MapToDto(updatedSubscription);
    }

    public async Task<bool> DeleteAsync(Guid id, string? userRole)
    {
        // Only Admin can delete subscriptions
        if (userRole != "Admin")
        {
            throw new UnauthorizedAccessException("Only administrators can delete subscriptions");
        }

        var result = await _subscriptionRepository.DeleteAsync(id);
        if (result)
        {
            _logger.LogInformation("Subscription deleted: {Id}", id);
        }

        return result;
    }

    private static SubscriptionDto MapToDto(Subscription subscription)
    {
        return new SubscriptionDto
        {
            Id = subscription.Id,
            UserId = subscription.UserId,
            User = subscription.User != null ? new UserDto
            {
                Id = subscription.User.Id,
                Email = subscription.User.Email,
                FirstName = subscription.User.FirstName,
                LastName = subscription.User.LastName,
                PhoneNumber = subscription.User.PhoneNumber,
                Role = subscription.User.Role,
                CreatedAt = subscription.User.CreatedAt
            } : null,
            Type = subscription.Type,
            Price = subscription.Price,
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            IsActive = subscription.IsActive,
            CreatedAt = subscription.CreatedAt
        };
    }
}
