namespace MedicalCertificate.WebAPI.Contracts;

public class RejectCertificateRequest
{
    public int RejectedByUserId { get; set; }
    public string Comment { get; set; } = string.Empty;
}