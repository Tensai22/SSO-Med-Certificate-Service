using MedicalCertificate.Domain.Entities;

namespace MedicalCertificate.Application.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByEmailWithRoleAsync(string email);
        Task<User?> GetByIdWithRoleAsync(int id);
        Task<IEnumerable<User>> GetAllWithRolesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<User?> GetByIdAsync(int id);

    }
}