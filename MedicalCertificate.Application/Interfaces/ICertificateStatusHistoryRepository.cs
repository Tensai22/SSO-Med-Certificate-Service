using MedicalCertificate.Domain.Entities;

namespace MedicalCertificate.Application.Interfaces;

public interface ICertificateStatusHistoryRepository
{
    Task AddAsync(CertificateStatusHistory history);
    Task<List<CertificateStatusHistory>> GetByCertificateIdAsync(int certificateId);
}