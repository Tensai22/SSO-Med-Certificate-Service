using MedicalCertificate.Application.DTOs;
using MedicalCertificate.Domain.Entities;
using KDS.Primitives.FluentResult;
using System.Threading.Tasks;

namespace MedicalCertificate.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserDto[]>> GetAllAsync();
        Task<Result<UserDto?>> GetByIdAsync(int id);
        Task<Result<UserDto?>> GetByEmailAsync(string email);
        Task<Result<UserDto>> CreateAsync(UserDto userDto, CancellationToken cancellationToken);
        Task<Result<UserDto>> UpdateAsync(int id, UserDto userDto);
        Task<Result<bool>> DeleteAsync(int id);
    }
}