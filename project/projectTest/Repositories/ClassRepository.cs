using Microsoft.EntityFrameworkCore;
using projectTest.Data;
using projectTest.Models.Entities;
using projectTest.Repositories.Interfaces;

namespace projectTest.Repositories;

public class ClassRepository : IClassRepository
{
    private readonly ApplicationDbContext _context;

    public ClassRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Class?> GetByIdAsync(Guid id)
    {
        return await _context.Classes
            .Include(c => c.Instructor)
            .Include(c => c.Bookings)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Class>> GetAllAsync()
    {
        return await _context.Classes
            .Include(c => c.Instructor)
            .Include(c => c.Bookings)
            .ToListAsync();
    }

    public async Task<List<Class>> GetFilteredAsync(string? search, Guid? instructorId, DateTime? startDate, DateTime? endDate, decimal? minPrice, decimal? maxPrice)
    {
        var query = _context.Classes
            .Include(c => c.Instructor)
            .Include(c => c.Bookings)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => 
                c.Name.Contains(search) || 
                c.Description.Contains(search));
        }

        if (instructorId.HasValue)
        {
            query = query.Where(c => c.InstructorId == instructorId.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(c => c.StartTime >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(c => c.EndTime <= endDate.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(c => c.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(c => c.Price <= maxPrice.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<int> GetTotalCountAsync(string? search, Guid? instructorId, DateTime? startDate, DateTime? endDate, decimal? minPrice, decimal? maxPrice)
    {
        var query = _context.Classes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => 
                c.Name.Contains(search) || 
                c.Description.Contains(search));
        }

        if (instructorId.HasValue)
        {
            query = query.Where(c => c.InstructorId == instructorId.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(c => c.StartTime >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(c => c.EndTime <= endDate.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(c => c.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(c => c.Price <= maxPrice.Value);
        }

        return await query.CountAsync();
    }

    public async Task<Class> CreateAsync(Class classEntity)
    {
        classEntity.CreatedAt = DateTime.UtcNow;
        _context.Classes.Add(classEntity);
        await _context.SaveChangesAsync();
        return classEntity;
    }

    public async Task<Class> UpdateAsync(Class classEntity)
    {
        classEntity.UpdatedAt = DateTime.UtcNow;
        _context.Classes.Update(classEntity);
        await _context.SaveChangesAsync();
        return classEntity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var classEntity = await _context.Classes.FindAsync(id);
        if (classEntity == null)
            return false;

        _context.Classes.Remove(classEntity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Classes.AnyAsync(c => c.Id == id);
    }
}
