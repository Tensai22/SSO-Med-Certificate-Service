using MedicalCertificate.Application.Interfaces;
using MedicalCertificate.Domain.Entities;
using MedicalCertificate.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using KDS.Primitives.FluentResult;

namespace MedicalCertificate.Infrastructure.Repositories;

public class CertificateRepository : Repository<Certificate>, ICertificateRepository
{
    private readonly AppDbContext _context;

    public CertificateRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Certificate>> GetAllWithSupervisorAsync()
    {
        return await BuildCertificateProfileQuery().ToListAsync();
    }

    public async Task<Certificate?> GetByIdWithSupervisorAsync(int id)
    {
        return await BuildCertificateProfileQuery().FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Certificate>> GetAllWithStatusAsync()
    {
        return await BuildCertificateProfileQuery()
            .Include(c => c.Status)
            .ToListAsync();
    }

    public async Task<Certificate?> GetByIdWithStatusAsync(int id)
    {
        return await BuildCertificateProfileQuery()
            .Include(c => c.Status)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task RemoveAsync(Certificate entity)
    {
        _context.Certificates.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> UpdateAsync(Certificate entity)
    {
        _context.Certificates.Update(entity);
        await _context.SaveChangesAsync();
        return Result.Success();
    }
    public async Task<List<Certificate>> GetByUserIdAsync(int userId)
    {
        return await BuildCertificateProfileQuery()
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }

    public async Task<Certificate?> GetByFilePathIdAsync(int filePathId)
    {
        return await _context.Certificates.FirstOrDefaultAsync(c => c.FilePathId == filePathId);
    }

    public async Task<IEnumerable<Certificate>> GetAllAsync()
    {
        return await BuildCertificateProfileQuery().ToListAsync();
    }

    private IQueryable<Certificate> BuildCertificateProfileQuery()
    {
        return _context.Certificates
            .Include(c => c.User)
                .ThenInclude(u => u.Student)
                    .ThenInclude(s => s.Speciality)
                        .ThenInclude(u => u!.Type)
            .Include(c => c.User)
                .ThenInclude(u => u.Student)
                    .ThenInclude(s => s.Speciality)
                        .ThenInclude(u => u!.Parent)
                            .ThenInclude(p => p!.Type)
            .Include(c => c.User)
                .ThenInclude(u => u.Student)
                    .ThenInclude(s => s.Speciality)
                        .ThenInclude(u => u!.Parent)
                            .ThenInclude(p => p!.Parent)
                                .ThenInclude(pp => pp!.Type);
    }
}
