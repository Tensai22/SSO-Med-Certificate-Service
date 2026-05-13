using MedicalCertificate.Domain.Entities;

namespace MedicalCertificate.Application.Interfaces
{
    public interface IEduUserRepository : IRepository<Edu_Users>
    {
        Task<Edu_Users?> GetByEmailAsync(string email);
        Task<Edu_Users?> GetByEmailWithRelationsAsync(string email);
        Task<Edu_Users?> GetByIdWithRelationsAsync(int id);
        Task<IEnumerable<Edu_Users>> GetAllWithRelationsAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
