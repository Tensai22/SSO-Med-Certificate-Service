using MedicalCertificate.Application.DTOs;
using KDS.Primitives.FluentResult;

namespace MedicalCertificate.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponseDto>> LoginAsync(LoginDTO loginDto);
    }
}