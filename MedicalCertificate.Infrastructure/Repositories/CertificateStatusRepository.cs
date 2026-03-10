using MedicalCertificate.Application.Interfaces;
using MedicalCertificate.Domain.Entities;
using MedicalCertificate.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace MedicalCertificate.Infrastructure.Repositories;

public class CertificateStatusRepository : ICertificateStatusRepository
{
    private readonly AppDbContext _context;

    public CertificateStatusRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CertificateStatus?> GetByIdAsync(int id)
    {
        return await _context.CertificateStatuses.FirstOrDefaultAsync(s => s.Id == id);
    }
}