using MedicalCertificate.Application.Interfaces;
using MedicalCertificate.Domain.Entities;
using MedicalCertificate.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace MedicalCertificate.Infrastructure.Repositories
{
    // TODO(copilot): these eager-loading paths are wider than the final certificate-only shape; trim them later.
    public class EduUserRepository : Repository<Edu_Users>, IEduUserRepository
    {
        private readonly AppDbContext _appDbContext;

        public EduUserRepository(AppDbContext context) : base(context)
        {
            _appDbContext = context;
        }

        public async Task<Edu_Users?> GetByEmailAsync(string email)
        {
            return await _appDbContext.Set<Edu_Users>()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Edu_Users?> GetByEmailWithRelationsAsync(string email)
        {
            return await _appDbContext.Set<Edu_Users>()
                .Include(u => u.User)
                .Include(u => u.Student)
                    .ThenInclude(s => s.Speciality)
                    .ThenInclude(o => o!.Type)
                .Include(u => u.Student)
                    .ThenInclude(s => s.Speciality)
                    .ThenInclude(o => o!.Parent)
                    .ThenInclude(p => p!.Type)
                .Include(u => u.Student)
                    .ThenInclude(s => s.Speciality)
                    .ThenInclude(o => o!.Parent)
                    .ThenInclude(p => p!.Parent)
                    .ThenInclude(pp => pp!.Type)
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Edu_Users?> GetByIdWithRelationsAsync(int id)
        {
            return await _appDbContext.Set<Edu_Users>()
                .Include(u => u.User)
                .Include(u => u.Student)
                    .ThenInclude(s => s.Speciality)
                    .ThenInclude(o => o!.Type)
                .Include(u => u.Student)
                    .ThenInclude(s => s.Speciality)
                    .ThenInclude(o => o!.Parent)
                    .ThenInclude(p => p!.Type)
                .Include(u => u.Student)
                    .ThenInclude(s => s.Speciality)
                    .ThenInclude(o => o!.Parent)
                    .ThenInclude(p => p!.Parent)
                    .ThenInclude(pp => pp!.Type)
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.ID == id);
        }

        public async Task<IEnumerable<Edu_Users>> GetAllWithRelationsAsync()
        {
            return await _appDbContext.Set<Edu_Users>()
                .Include(u => u.User)
                .Include(u => u.Student)
                    .ThenInclude(s => s.Speciality)
                    .ThenInclude(o => o!.Type)
                .Include(u => u.Student)
                    .ThenInclude(s => s.Speciality)
                    .ThenInclude(o => o!.Parent)
                    .ThenInclude(p => p!.Type)
                .Include(u => u.Student)
                    .ThenInclude(s => s.Speciality)
                    .ThenInclude(o => o!.Parent)
                    .ThenInclude(p => p!.Parent)
                    .ThenInclude(pp => pp!.Type)
                .Include(u => u.Employee)
                .ToListAsync();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _appDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
