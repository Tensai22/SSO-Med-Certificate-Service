using MedicalCertificate.Application.Interfaces;
using MedicalCertificate.Domain.Entities;
using MedicalCertificate.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;


namespace MedicalCertificate.Infrastructure.Repositories
{
    public class RoleRepository(AppDbContext context) : Repository<Role>(context), IRoleRepository
    {
        public async Task<Role?> GetByNameAsync(string name)
        {
            return await context.Roles.FirstOrDefaultAsync(r => r.Name == name);
        }
    }
}