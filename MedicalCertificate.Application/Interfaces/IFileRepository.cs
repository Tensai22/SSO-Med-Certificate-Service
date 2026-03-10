using MedicalCertificate.Domain.Entities;

namespace MedicalCertificate.Application.Interfaces;

public interface IFileRepository
{
    Task<StoredFile?> GetByNameAsync(string fileName);
    Task MarkAsDeletedAsync(string fileName);
    Task CreateAsync(StoredFile file);
    Task UpdateAsync(StoredFile file);
}