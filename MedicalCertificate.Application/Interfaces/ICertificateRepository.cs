using MedicalCertificate.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KDS.Primitives.FluentResult;

namespace MedicalCertificate.Application.Interfaces;

    public interface ICertificateRepository : IRepository<Certificate>
    {
        Task<IEnumerable<Certificate>> GetAllWithStatusAsync();
        Task<Certificate?> GetByIdWithStatusAsync(int id);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(Certificate certificate);
        Task RemoveAsync(Certificate certificate);
    }

