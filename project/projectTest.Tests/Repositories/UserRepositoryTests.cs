using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using projectTest.Data;
using projectTest.Models.Entities;
using projectTest.Repositories;
using Xunit;

namespace projectTest.Tests.Repositories;

public class UserRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new UserRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateUser()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = "hash",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "1234567890",
            Role = "User"
        };

        // Act
        var result = await _repository.CreateAsync(user);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        result.Email.Should().Be(user.Email);
        var dbUser = await _context.Users.FindAsync(user.Id);
        dbUser.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenExists()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = "hash",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "1234567890",
            Role = "User"
        };
        await _repository.CreateAsync(user);

        // Act
        var result = await _repository.GetByIdAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUser_WhenExists()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = "hash",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "1234567890",
            Role = "User"
        };
        await _repository.CreateAsync(user);

        // Act
        var result = await _repository.GetByEmailAsync("test@example.com");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = "hash",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "1234567890",
            Role = "User"
        };
        await _repository.CreateAsync(user);
        user.FirstName = "Updated";

        // Act
        var result = await _repository.UpdateAsync(user);

        // Assert
        result.FirstName.Should().Be("Updated");
        var dbUser = await _context.Users.FindAsync(user.Id);
        dbUser!.FirstName.Should().Be("Updated");
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteUser_WhenExists()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = "hash",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "1234567890",
            Role = "User"
        };
        await _repository.CreateAsync(user);

        // Act
        var result = await _repository.DeleteAsync(user.Id);

        // Assert
        result.Should().BeTrue();
        var dbUser = await _context.Users.FindAsync(user.Id);
        dbUser.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotExists()
    {
        // Act
        var result = await _repository.DeleteAsync(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenExists()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = "hash",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "1234567890",
            Role = "User"
        };
        await _repository.CreateAsync(user);

        // Act
        var result = await _repository.ExistsAsync(user.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenNotExists()
    {
        // Act
        var result = await _repository.ExistsAsync(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var user1 = new User
        {
            Id = Guid.NewGuid(),
            Email = "test1@example.com",
            PasswordHash = "hash",
            FirstName = "Test1",
            LastName = "User",
            PhoneNumber = "1234567890",
            Role = "User"
        };
        var user2 = new User
        {
            Id = Guid.NewGuid(),
            Email = "test2@example.com",
            PasswordHash = "hash",
            FirstName = "Test2",
            LastName = "User",
            PhoneNumber = "1234567890",
            Role = "User"
        };
        await _repository.CreateAsync(user1);
        await _repository.CreateAsync(user2);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
