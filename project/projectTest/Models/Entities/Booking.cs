namespace projectTest.Models.Entities;

// Many-to-many relationship between User and Class
public class Booking
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ClassId { get; set; }
    public DateTime BookingDate { get; set; }
    public string Status { get; set; } = "Active"; // Active, Cancelled, Completed
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Class Class { get; set; } = null!;
}
