using Microsoft.EntityFrameworkCore;
using projectTest.Data;
using projectTest.Models.Entities;
using projectTest.Repositories.Interfaces;

namespace projectTest.Repositories;

public class InstructorRepository : IInstructorRepository
{
    private readonly ApplicationDbContext _context;

    public InstructorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Instructor?> GetByIdAsync(Guid id)
    {
        return await _context.Instructors
            .Include(i => i.Classes)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<List<Instructor>> GetAllAsync()
    {
        return await _context.Instructors
            .Include(i => i.Classes)
            .ToListAsync();
    }

    public async Task<Instructor> CreateAsync(Instructor instructor)
    {
        instructor.CreatedAt = DateTime.UtcNow;
        _context.Instructors.Add(instructor);
        await _context.SaveChangesAsync();
        return instructor;
    }

    public async Task<Instructor> UpdateAsync(Instructor instructor)
    {
        instructor.UpdatedAt = DateTime.UtcNow;
        _context.Instructors.Update(instructor);
        await _context.SaveChangesAsync();
        return instructor;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var instructor = await _context.Instructors.FindAsync(id);
        if (instructor == null)
            return false;

        _context.Instructors.Remove(instructor);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Instructors.AnyAsync(i => i.Id == id);
    }
}
