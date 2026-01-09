namespace projectTest.Models.DTO;

public class ClassDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MaxParticipants { get; set; }
    public int CurrentParticipants { get; set; }
    public decimal Price { get; set; }
    public Guid InstructorId { get; set; }
    public InstructorDto? Instructor { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateClassDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MaxParticipants { get; set; }
    public decimal Price { get; set; }
    public Guid InstructorId { get; set; }
}

public class UpdateClassDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? MaxParticipants { get; set; }
    public decimal? Price { get; set; }
    public Guid? InstructorId { get; set; }
}

public class ClassFilterDto
{
    public string? Search { get; set; }
    public Guid? InstructorId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? MinPrice { get; set; }
    public int? MaxPrice { get; set; }
}
