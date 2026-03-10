using MedicalCertificate.Domain.Entities;

namespace MedicalCertificate.Application.Interfaces;

public interface IJwtProvider
{
    string GenerateToken(User user);
}