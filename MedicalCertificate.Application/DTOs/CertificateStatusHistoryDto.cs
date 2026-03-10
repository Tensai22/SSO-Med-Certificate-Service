namespace MedicalCertificate.Application.DTOs;

public class CertificateStatusHistoryDto
{
    public string Status { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
    public string ChangedByUser { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
}