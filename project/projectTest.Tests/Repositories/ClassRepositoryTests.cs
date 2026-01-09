using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using projectTest.Data;
using projectTest.Models.Entities;
using projectTest.Repositories;
using Xunit;

namespace projectTest.Tests.Repositories;

public class ClassRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ClassRepository _repository;
    private readonly InstructorRepository _instructorRepository;

    public ClassRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new ClassRepository(_context);
        _instructorRepository = new InstructorRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateClass()
    {
        // Arrange
        var instructor = new Instructor
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "1234567890",
            Specialization = "Pilates",
            ExperienceYears = 5
        };
        await _instructorRepository.CreateAsync(instructor);

        var classEntity = new Class
        {
            Id = Guid.NewGuid(),
            Name = "Morning Pilates",
            Description = "Morning class",
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
            MaxParticipants = 10,
            CurrentParticipants = 0,
            Price = 50,
            InstructorId = instructor.Id
        };

        // Act
        var result = await _repository.CreateAsync(classEntity);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(classEntity.Id);
        result.Name.Should().Be(classEntity.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnClass_WhenExists()
    {
        // Arrange
        var instructor = new Instructor
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "1234567890",
            Specialization = "Pilates",
            ExperienceYears = 5
        };
        await _instructorRepository.CreateAsync(instructor);

        var classEntity = new Class
        {
            Id = Guid.NewGuid(),
            Name = "Morning Pilates",
            Description = "Morning class",
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
            MaxParticipants = 10,
            CurrentParticipants = 0,
            Price = 50,
            InstructorId = instructor.Id
        };
        await _repository.CreateAsync(classEntity);

        // Act
        var result = await _repository.GetByIdAsync(classEntity.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(classEntity.Id);
        result.Name.Should().Be(classEntity.Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateClass()
    {
        // Arrange
        var instructor = new Instructor
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "1234567890",
            Specialization = "Pilates",
            ExperienceYears = 5
        };
        await _instructorRepository.CreateAsync(instructor);

        var classEntity = new Class
        {
            Id = Guid.NewGuid(),
            Name = "Morning Pilates",
            Description = "Morning class",
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
            MaxParticipants = 10,
            CurrentParticipants = 0,
            Price = 50,
            InstructorId = instructor.Id
        };
        await _repository.CreateAsync(classEntity);
        classEntity.Name = "Updated Name";

        // Act
        var result = await _repository.UpdateAsync(classEntity);

        // Assert
        result.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteClass_WhenExists()
    {
        // Arrange
        var instructor = new Instructor
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "1234567890",
            Specialization = "Pilates",
            ExperienceYears = 5
        };
        await _instructorRepository.CreateAsync(instructor);

        var classEntity = new Class
        {
            Id = Guid.NewGuid(),
            Name = "Morning Pilates",
            Description = "Morning class",
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
            MaxParticipants = 10,
            CurrentParticipants = 0,
            Price = 50,
            InstructorId = instructor.Id
        };
        await _repository.CreateAsync(classEntity);

        // Act
        var result = await _repository.DeleteAsync(classEntity.Id);

        // Assert
        result.Should().BeTrue();
        var dbClass = await _context.Classes.FindAsync(classEntity.Id);
        dbClass.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
