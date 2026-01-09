namespace projectTest.Models.DTO;

public class BookingDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public UserDto? User { get; set; }
    public Guid ClassId { get; set; }
    public ClassDto? Class { get; set; }
    public DateTime BookingDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateBookingDto
{
    public Guid ClassId { get; set; }
}

public class UpdateBookingDto
{
    public string? Status { get; set; }
}
