namespace MedicalCertificate.Application.DTOs;
using Microsoft.AspNetCore.Http;

public class CreateCertificateDto 
{
    public string FullName { get; set; } = string.Empty;
    public string Iin { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Clinic { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public IFormFile File { get; set; } = null!;
}