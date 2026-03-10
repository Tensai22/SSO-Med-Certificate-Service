using MedicalCertificate.Application.Interfaces;
using MedicalCertificate.Domain.Entities;
using MedicalCertificate.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace MedicalCertificate.Infrastructure.Repositories;

public class CertificateStatusHistoryRepository : ICertificateStatusHistoryRepository
{
    private readonly AppDbContext _context;

    public CertificateStatusHistoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(CertificateStatusHistory history)
    {
        await _context.CertificateStatusHistories.AddAsync(history);
    }

    public async Task<List<CertificateStatusHistory>> GetByCertificateIdAsync(int certificateId)
    {
        return await _context.CertificateStatusHistories
            .Where(h => h.CertificateId == certificateId)
            .ToListAsync();
    }

    public async Task<CertificateStatusHistory?> GetByIdAsync(int id)
    {
        return await _context.CertificateStatusHistories.FirstOrDefaultAsync(s => s.Id == id);
    }

}
