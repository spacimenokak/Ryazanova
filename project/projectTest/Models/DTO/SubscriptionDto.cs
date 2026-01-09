namespace projectTest.Models.DTO;

public class SubscriptionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public UserDto? User { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateSubscriptionDto
{
    public Guid UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class UpdateSubscriptionDto
{
    public bool? IsActive { get; set; }
    public DateTime? EndDate { get; set; }
}
