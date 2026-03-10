using MedicalCertificate.Domain.Entities;
using System.Threading.Tasks;

namespace MedicalCertificate.Application.Interfaces
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<Role?> GetByNameAsync(string name);
    }
}