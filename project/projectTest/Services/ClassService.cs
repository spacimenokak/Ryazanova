using projectTest.Models.DTO;
using projectTest.Models.Entities;
using projectTest.Repositories.Interfaces;
using projectTest.Services.Interfaces;

namespace projectTest.Services;

public class ClassService : IClassService
{
    private readonly IClassRepository _classRepository;
    private readonly IInstructorRepository _instructorRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<ClassService> _logger;

    public ClassService(
        IClassRepository classRepository,
        IInstructorRepository instructorRepository,
        ICacheService cacheService,
        ILogger<ClassService> logger)
    {
        _classRepository = classRepository;
        _instructorRepository = instructorRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ClassDto?> GetByIdAsync(Guid id)
    {
        var cacheKey = $"class:{id}";
        var cached = await _cacheService.GetAsync<ClassDto>(cacheKey);
        if (cached != null)
            return cached;

        var classEntity = await _classRepository.GetByIdAsync(id);
        if (classEntity == null)
            return null;

        var dto = MapToDto(classEntity);
        await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5));

        return dto;
    }

    public async Task<PagedResponseDto<ClassDto>> GetFilteredAsync(ClassFilterDto filter, int page, int pageSize)
    {
        var cacheKey = $"classes:page:{page}:size:{pageSize}:filter:{filter.GetHashCode()}";
        var cached = await _cacheService.GetAsync<PagedResponseDto<ClassDto>>(cacheKey);
        if (cached != null)
            return cached;

        var classes = await _classRepository.GetFilteredAsync(
            filter.Search,
            filter.InstructorId,
            filter.StartDate,
            filter.EndDate,
            filter.MinPrice,
            filter.MaxPrice);

        var total = await _classRepository.GetTotalCountAsync(
            filter.Search,
            filter.InstructorId,
            filter.StartDate,
            filter.EndDate,
            filter.MinPrice,
            filter.MaxPrice);

        var pagedClasses = classes
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(MapToDto)
            .ToList();

        var response = new PagedResponseDto<ClassDto>
        {
            Items = pagedClasses,
            Total = total,
            Page = page,
            PageSize = pageSize
        };

        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(2));

        return response;
    }

    public async Task<ClassDto> CreateAsync(CreateClassDto dto, string? userRole)
    {
        // Only Admin and Manager can create classes
        if (userRole != "Admin" && userRole != "Manager")
        {
            throw new UnauthorizedAccessException("You don't have permission to create classes");
        }

        if (!await _instructorRepository.ExistsAsync(dto.InstructorId))
        {
            throw new KeyNotFoundException("Instructor not found");
        }

        if (dto.StartTime >= dto.EndTime)
        {
            throw new ArgumentException("Start time must be before end time");
        }

        var classEntity = new Class
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            MaxParticipants = dto.MaxParticipants,
            CurrentParticipants = 0,
            Price = dto.Price,
            InstructorId = dto.InstructorId
        };

        var createdClass = await _classRepository.CreateAsync(classEntity);
        _logger.LogInformation("Class created: {Name}", createdClass.Name);

        // Invalidate cache
        await _cacheService.RemoveByPatternAsync("classes:*");

        return MapToDto(createdClass);
    }

    public async Task<ClassDto> UpdateAsync(Guid id, UpdateClassDto dto, string? userRole)
    {
        // Only Admin and Manager can update classes
        if (userRole != "Admin" && userRole != "Manager")
        {
            throw new UnauthorizedAccessException("You don't have permission to update classes");
        }

        var classEntity = await _classRepository.GetByIdAsync(id);
        if (classEntity == null)
            throw new KeyNotFoundException("Class not found");

        if (dto.InstructorId.HasValue && !await _instructorRepository.ExistsAsync(dto.InstructorId.Value))
        {
            throw new KeyNotFoundException("Instructor not found");
        }

        if (dto.StartTime.HasValue && dto.EndTime.HasValue && dto.StartTime >= dto.EndTime)
        {
            throw new ArgumentException("Start time must be before end time");
        }

        if (dto.Name != null)
            classEntity.Name = dto.Name;
        if (dto.Description != null)
            classEntity.Description = dto.Description;
        if (dto.StartTime.HasValue)
            classEntity.StartTime = dto.StartTime.Value;
        if (dto.EndTime.HasValue)
            classEntity.EndTime = dto.EndTime.Value;
        if (dto.MaxParticipants.HasValue)
            classEntity.MaxParticipants = dto.MaxParticipants.Value;
        if (dto.Price.HasValue)
            classEntity.Price = dto.Price.Value;
        if (dto.InstructorId.HasValue)
            classEntity.InstructorId = dto.InstructorId.Value;

        var updatedClass = await _classRepository.UpdateAsync(classEntity);
        _logger.LogInformation("Class updated: {Name}", updatedClass.Name);

        // Invalidate cache
        await _cacheService.RemoveByPatternAsync("classes:*");
        await _cacheService.RemoveAsync($"class:{id}");

        return MapToDto(updatedClass);
    }

    public async Task<bool> DeleteAsync(Guid id, string? userRole)
    {
        // Only Admin can delete classes
        if (userRole != "Admin")
        {
            throw new UnauthorizedAccessException("Only administrators can delete classes");
        }

        var result = await _classRepository.DeleteAsync(id);
        if (result)
        {
            _logger.LogInformation("Class deleted: {Id}", id);
            // Invalidate cache
            await _cacheService.RemoveByPatternAsync("classes:*");
            await _cacheService.RemoveAsync($"class:{id}");
        }

        return result;
    }

    private static ClassDto MapToDto(Class classEntity)
    {
        return new ClassDto
        {
            Id = classEntity.Id,
            Name = classEntity.Name,
            Description = classEntity.Description,
            StartTime = classEntity.StartTime,
            EndTime = classEntity.EndTime,
            MaxParticipants = classEntity.MaxParticipants,
            CurrentParticipants = classEntity.CurrentParticipants,
            Price = classEntity.Price,
            InstructorId = classEntity.InstructorId,
            Instructor = classEntity.Instructor != null ? new InstructorDto
            {
                Id = classEntity.Instructor.Id,
                FirstName = classEntity.Instructor.FirstName,
                LastName = classEntity.Instructor.LastName,
                Email = classEntity.Instructor.Email,
                PhoneNumber = classEntity.Instructor.PhoneNumber,
                Specialization = classEntity.Instructor.Specialization,
                ExperienceYears = classEntity.Instructor.ExperienceYears,
                CreatedAt = classEntity.Instructor.CreatedAt
            } : null,
            CreatedAt = classEntity.CreatedAt
        };
    }
}
