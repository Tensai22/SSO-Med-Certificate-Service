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
        return await _context.Certificates.Include(t => t.User).ToListAsync();
    }

    public async Task<Certificate?> GetByIdWithSupervisorAsync(int id)
    {
        return await _context.Certificates.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Certificate>> GetAllWithStatusAsync()
    {
        return await _context.Certificates
            .Include(c => c.Status)
            .Include(c => c.User)
            .ToListAsync();
    }

    public async Task<Certificate?> GetByIdWithStatusAsync(int id)
    {
        return await _context.Certificates
            .Include(c => c.Status)
            .Include(c => c.User)
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
}

