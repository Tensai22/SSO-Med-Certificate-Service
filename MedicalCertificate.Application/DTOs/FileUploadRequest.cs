using Microsoft.AspNetCore.Http;

namespace MedicalCertificate.Application.DTOs;
public class FileUploadRequest
{
    public IFormFile File { get; set; } = default!;
}