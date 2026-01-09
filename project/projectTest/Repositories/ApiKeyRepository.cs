using Microsoft.EntityFrameworkCore;
using projectTest.Data;
using projectTest.Models.Entities;
using projectTest.Repositories.Interfaces;

namespace projectTest.Repositories;

public class ApiKeyRepository : IApiKeyRepository
{
    private readonly ApplicationDbContext _context;

    public ApiKeyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiKey?> GetByIdAsync(Guid id)
    {
        return await _context.ApiKeys.FindAsync(id);
    }

    public async Task<ApiKey?> GetByKeyAsync(string key)
    {
        return await _context.ApiKeys
            .FirstOrDefaultAsync(k => k.Key == key);
    }

    public async Task<List<ApiKey>> GetAllAsync()
    {
        return await _context.ApiKeys.ToListAsync();
    }

    public async Task<ApiKey> CreateAsync(ApiKey apiKey)
    {
        apiKey.CreatedAt = DateTime.UtcNow;
        _context.ApiKeys.Add(apiKey);
        await _context.SaveChangesAsync();
        return apiKey;
    }

    public async Task<ApiKey> UpdateAsync(ApiKey apiKey)
    {
        _context.ApiKeys.Update(apiKey);
        await _context.SaveChangesAsync();
        return apiKey;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var apiKey = await _context.ApiKeys.FindAsync(id);
        if (apiKey == null)
            return false;

        _context.ApiKeys.Remove(apiKey);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsValidAsync(string key)
    {
        var apiKey = await GetByKeyAsync(key);
        if (apiKey == null)
            return false;

        if (!apiKey.IsActive)
            return false;

        if (apiKey.ExpiresAt.HasValue && apiKey.ExpiresAt.Value < DateTime.UtcNow)
            return false;

        return true;
    }
}
