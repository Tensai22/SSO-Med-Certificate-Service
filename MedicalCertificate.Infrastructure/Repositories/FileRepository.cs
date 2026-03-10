using Microsoft.EntityFrameworkCore;
using MedicalCertificate.Application.Interfaces;
using MedicalCertificate.Domain.Entities;
using MedicalCertificate.Infrastructure.Services;

namespace MedicalCertificate.Infrastructure.Repositories;

public class FileRepository : IFileRepository
{
    private readonly AppDbContext _context;

    public FileRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<StoredFile?> GetByNameAsync(string fileName)
    {
        return await _context.StoredFiles
            .FirstOrDefaultAsync(f => f.Name == fileName && !f.IsDeleted);
    }

    public async Task MarkAsDeletedAsync(string fileName)
    {
        var file = await _context.StoredFiles
            .FirstOrDefaultAsync(f => f.Name == fileName);

        if (file is not null && !file.IsDeleted)
        {
            file.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
    }
    public async Task CreateAsync(StoredFile file)
    {
        _context.StoredFiles.Add(file);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(StoredFile file)
    {
        _context.StoredFiles.Update(file);
        await _context.SaveChangesAsync();
    }
}