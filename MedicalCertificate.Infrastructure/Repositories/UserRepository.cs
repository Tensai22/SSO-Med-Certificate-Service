using MedicalCertificate.Application.Interfaces;
using MedicalCertificate.Domain.Entities;
using MedicalCertificate.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicalCertificate.Application.DTOs;

namespace MedicalCertificate.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly AppDbContext _appDbContext;

        public UserRepository(AppDbContext context) : base(context)
        {
            _appDbContext = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _appDbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        
        public async Task<User?> GetByEmailWithRoleAsync(string email)
        {
            return await _appDbContext.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        
        public async Task<User?> GetByIdWithRoleAsync(int id)
        {
            
            return await _appDbContext.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
        
        public async Task<IEnumerable<User>> GetAllWithRolesAsync()
        {
            return await _appDbContext.Users
                .Include(u => u.Role)
                .ToListAsync();
        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _appDbContext.Users.ToListAsync();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _appDbContext.SaveChangesAsync(cancellationToken);
        }
        
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _appDbContext.Users.FindAsync(id);
        }
        
    }
}