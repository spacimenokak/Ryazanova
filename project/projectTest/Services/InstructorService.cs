using projectTest.Models.DTO;
using projectTest.Models.Entities;
using projectTest.Repositories.Interfaces;
using projectTest.Services.Interfaces;

namespace projectTest.Services;

public class InstructorService : IInstructorService
{
    private readonly IInstructorRepository _instructorRepository;
    private readonly ILogger<InstructorService> _logger;

    public InstructorService(IInstructorRepository instructorRepository, ILogger<InstructorService> logger)
    {
        _instructorRepository = instructorRepository;
        _logger = logger;
    }

    public async Task<InstructorDto?> GetByIdAsync(Guid id)
    {
        var instructor = await _instructorRepository.GetByIdAsync(id);
        if (instructor == null)
            return null;

        return MapToDto(instructor);
    }

    public async Task<List<InstructorDto>> GetAllAsync()
    {
        var instructors = await _instructorRepository.GetAllAsync();
        return instructors.Select(MapToDto).ToList();
    }

    public async Task<InstructorDto> CreateAsync(CreateInstructorDto dto, string? userRole)
    {
        // Only Admin and Manager can create instructors
        if (userRole != "Admin" && userRole != "Manager")
        {
            throw new UnauthorizedAccessException("You don't have permission to create instructors");
        }

        var instructor = new Instructor
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Specialization = dto.Specialization,
            ExperienceYears = dto.ExperienceYears
        };

        var createdInstructor = await _instructorRepository.CreateAsync(instructor);
        _logger.LogInformation("Instructor created: {Email}", createdInstructor.Email);

        return MapToDto(createdInstructor);
    }

    public async Task<InstructorDto> UpdateAsync(Guid id, UpdateInstructorDto dto, string? userRole)
    {
        // Only Admin and Manager can update instructors
        if (userRole != "Admin" && userRole != "Manager")
        {
            throw new UnauthorizedAccessException("You don't have permission to update instructors");
        }

        var instructor = await _instructorRepository.GetByIdAsync(id);
        if (instructor == null)
            throw new KeyNotFoundException("Instructor not found");

        if (dto.FirstName != null)
            instructor.FirstName = dto.FirstName;
        if (dto.LastName != null)
            instructor.LastName = dto.LastName;
        if (dto.Email != null)
            instructor.Email = dto.Email;
        if (dto.PhoneNumber != null)
            instructor.PhoneNumber = dto.PhoneNumber;
        if (dto.Specialization != null)
            instructor.Specialization = dto.Specialization;
        if (dto.ExperienceYears.HasValue)
            instructor.ExperienceYears = dto.ExperienceYears.Value;

        var updatedInstructor = await _instructorRepository.UpdateAsync(instructor);
        _logger.LogInformation("Instructor updated: {Email}", updatedInstructor.Email);

        return MapToDto(updatedInstructor);
    }

    public async Task<bool> DeleteAsync(Guid id, string? userRole)
    {
        // Only Admin can delete instructors
        if (userRole != "Admin")
        {
            throw new UnauthorizedAccessException("Only administrators can delete instructors");
        }

        var result = await _instructorRepository.DeleteAsync(id);
        if (result)
        {
            _logger.LogInformation("Instructor deleted: {Id}", id);
        }

        return result;
    }

    private static InstructorDto MapToDto(Instructor instructor)
    {
        return new InstructorDto
        {
            Id = instructor.Id,
            FirstName = instructor.FirstName,
            LastName = instructor.LastName,
            Email = instructor.Email,
            PhoneNumber = instructor.PhoneNumber,
            Specialization = instructor.Specialization,
            ExperienceYears = instructor.ExperienceYears,
            CreatedAt = instructor.CreatedAt
        };
    }
}
