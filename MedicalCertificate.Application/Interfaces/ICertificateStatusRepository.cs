using MedicalCertificate.Domain.Entities;

namespace MedicalCertificate.Application.Interfaces;

public interface ICertificateStatusRepository
{
    Task<CertificateStatus?> GetByIdAsync(int id);
}